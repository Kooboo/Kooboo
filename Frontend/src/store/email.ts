import { defineStore } from "pinia";
import type {
  EmailSearchModel,
  EmailFolder,
  SelectedEmail,
  EmailMessage,
} from "@/api/mail/types";
import type { Address, EmailItem } from "@/api/mail/types";
import {
  deleteEmail as deleteEmailApi,
  getAddressList,
  markReads as markReadApi,
  moveEmail as moveEmailApi,
  getMore,
  LatestMsgId,
  getEmailFolderList,
  advancedSearchMail,
} from "@/api/mail";
import type { EmailDomain } from "@/api/console/types";
import { getDomainList } from "@/api/console";
import { computed, nextTick, ref, watch } from "vue";
import { useRoute } from "vue-router";
import ImageIcon from "@/assets/images/attachment_image.svg";
import MediaIcon from "@/assets/images/attachment_media.svg";
import ZipIcon from "@/assets/images/attachment_rar.svg";
import DefaultIcon from "@/assets/images/attachment_default.svg";
import DocumentIcon from "@/assets/images/attachment_document.svg";

export const useEmailStore = defineStore("emailStore", () => {
  const route = useRoute();
  const isLoadAddress = ref(false);
  const domainList = ref<EmailDomain[]>([]);
  const addressList = ref<Address[]>([]);
  const folderList = ref<EmailFolder[]>([]);
  const folderMenuTree = ref<EmailFolder[]>([]);

  const defaultAddress = ref();
  const sentAddressList = ref<Address[]>([]);
  const messageList = ref<EmailItem[]>([]);
  const address = ref<string>();
  const folder = ref<string>("");
  const inboxUnreadCount = ref();
  const sentUnreadCount = ref();
  const folderUnreadCount = ref();
  const isShowNewMessageLogo = localStorage.getItem("isHasNewMessage");
  const selectedMessage = ref<SelectedEmail[]>([]);
  const firstActiveMenu = ref();
  const messageKeyword = ref();
  const searchModel = ref<EmailSearchModel | null>();
  const searchEmailsCount = ref(0);
  const preLatestId = ref();
  const notFolderOptions = ["inbox", "sent", "drafts", "spam", "trash"];

  // 上一次查看的邮件id
  const justViewMessageId = ref(0);

  const activeName = computed(() => {
    // 从搜索列表进入详情页时，不激活任何菜单
    if (route.query.flag) return;
    if (firstActiveMenu.value) return firstActiveMenu.value;
    let menuName = route.name ?? "";
    let menuQuery = "";

    if (route.query.folder) {
      menuName = route.query.folder.toString().toLowerCase();
    }
    if (route.name === "folder" || route.query.folder === "folder") {
      menuQuery = (route.query.folderName as string) ?? "";
    } else {
      menuQuery = (route.query.address as string) ?? "";
    }

    return menuName.toString() + menuQuery.toString();
  });

  // 把文件夹列表转为树状结构
  function getFolderTree(data: EmailFolder[]): EmailFolder[] {
    const folderTree: EmailFolder[] = [];

    data.forEach((item) => {
      const parts = item.name.split("/");
      let currentLevel = folderTree;
      let currentName = "";

      parts.forEach((part, index) => {
        currentName = currentName ? `${currentName}/${part}` : part;
        const existingItem = currentLevel.find((el) => el.name === currentName);
        if (existingItem) {
          currentLevel = existingItem.items!;
        } else {
          const newItem = {
            id: item.id,
            name: currentName,
            count: index === parts.length - 1 ? item.count : 0,
            displayName: index === 0 ? part : part,
            unRead: index === parts.length - 1 ? item.unRead : 0,
            floor: index,
            items: [],
          };
          currentLevel.push(newItem);
          currentLevel = newItem.items;
        }
      });
    });

    return folderTree;
  }

  const getMoreMessageQuery = computed(() => {
    const query =
      route.name === "folder" || route.query.folder === "folder"
        ? route.query.folderName?.toString()
          ? route.query.folderName?.toString()
          : ""
        : undefined;
    return query;
  });

  const loadDomains = async () => {
    domainList.value = await getDomainList();
  };

  const loadSentAddressList = async () => {
    sentAddressList.value = await getAddressList("sent");
    sentUnreadCount.value = sentAddressList.value
      .map((i) => i.unRead)
      .reduce((p, c) => {
        return p + c;
      }, 0);
  };

  const loadInboxAddressList = async (hiddenLoading?: boolean) => {
    addressList.value = await getAddressList(
      undefined,
      hiddenLoading ? true : undefined
    );
    defaultAddress.value = addressList.value.filter(
      (f) => f.isDefault && f.addressType !== "Wildcard"
    )[0]?.address;

    inboxUnreadCount.value = addressList.value
      .map((i) => i.unRead)
      .reduce((p, c) => {
        return p + c;
      }, 0);
  };

  //获取邮箱未读数量的方法
  const loadAddress = async (folder?: string) => {
    if (!folder) {
      await loadSentAddressList();
      await loadInboxAddressList();
    } else if (folder === "sent") {
      await loadSentAddressList();
    } else if (folder === "inbox" || folder === "addresses") {
      await loadInboxAddressList();
    }
  };

  const loadFolders = async () => {
    folderList.value = await getEmailFolderList();
    folderUnreadCount.value = folderList.value
      .map((i) => i.unRead)
      .reduce((p, c) => {
        return p + c;
      }, 0);
    folderMenuTree.value = getFolderTree(folderList.value);
  };

  const loadMessage = async () => {
    const lastMessageId =
      messageList.value[messageList.value?.length - 1]?.id ?? -1;
    let result;
    if (route.query.flag === "advanced" && searchModel.value) {
      // result = await advancedSearchMail(lastMessageId, searchModel.value);
    } else {
      result = await getMore(
        getMoreMessageQuery.value ?? folder.value,
        getMoreMessageQuery.value ? undefined : address.value,
        lastMessageId,
        messageKeyword.value || ""
      );
    }
    if (result) {
      messageList.value.push(...result);
    }

    // 如果更新了inbox的邮件，就重置preLatestId
    if (folder.value === "inbox") {
      preLatestId.value = await LatestMsgId();
    }
  };

  const reload = async (loadSentAndInboxAddress?: boolean) => {
    const oldLength = messageList.value.length;
    const newMessageList: EmailItem[] = [];
    while (oldLength) {
      const lastMessageId = newMessageList[newMessageList.length - 1]?.id ?? -1;
      let result;
      if (route.query.flag === "advanced" && searchModel.value) {
        result = await advancedSearchMail(lastMessageId, searchModel.value);
      } else {
        result = await getMore(
          getMoreMessageQuery.value ?? folder.value,
          getMoreMessageQuery.value ? undefined : address.value,
          lastMessageId,
          messageKeyword.value || "",
          true
        );
      }
      const data = result;

      if (!result.length) {
        break;
      }
      newMessageList.push(...data);
      if (newMessageList.length >= oldLength) {
        break;
      }
      if (oldLength <= 30) {
        break;
      }
    }

    // 如果更新了inbox的邮件，就重置preLatestId
    if (folder.value === "inbox") {
      preLatestId.value = await LatestMsgId();
    }
    messageList.value = newMessageList;

    //如果loadSentAndInboxAddress是true,则inbox和sent的未读数接口都请求,否则只请求当前folder的未读数接口
    if (loadSentAndInboxAddress) {
      loadAddress();
    } else if (loadSentAndInboxAddress === false) {
      loadAddress(folder.value);
    }
  };

  const refreshMessageList = async (advancedSearch = "") => {
    const newMessageList: EmailItem[] = [];
    let result;

    if (advancedSearch === "advanced" && searchModel.value) {
      result = await advancedSearchMail(-1, searchModel.value);
    } else {
      result = await getMore(
        getMoreMessageQuery.value ?? folder.value,
        getMoreMessageQuery.value ? undefined : address.value,
        -1,
        messageKeyword.value || ""
      );
    }
    // searchEmailsCount.value = result.count;

    // 如果更新了inbox的邮件，就重置preLatestId
    if (folder.value === "inbox") {
      preLatestId.value = await LatestMsgId();
    }
    newMessageList.push(...result);
    messageList.value = newMessageList;
    if (!messageList.value.length) return;
    await nextTick();
    document
      .getElementById(messageList.value[0].id.toString())
      ?.scrollIntoView();
  };

  const markRead = async (ids: number[], value: boolean) => {
    for (let i = 0; i < ids.length; i++) {
      messageList.value.forEach((item) => {
        if (item.id === ids[i]) {
          item.read = value;
        }
      });
    }
    await markReadApi(ids, value);
  };

  // 更新未读数
  const updateUnreadCount = (
    messages: SelectedEmail[],
    targetFolder: string
  ) => {
    const isHasUnreadMessage = messages.find(
      (f: SelectedEmail) => f.read === false
    );

    // 是否含有属于inbox的未读邮件
    const isHasUnreadInboxMessage = messages.find(
      (f: SelectedEmail) => f.folderName === "inbox" && f.read === false
    );

    // 是否含有属于sent的未读邮件
    const isHasUnreadSentMessage = messages.find(
      (f: SelectedEmail) => f.folderName === "sent" && f.read === false
    );

    // 是否含有属于folder的未读邮件
    const isHasUnreadFolderMessage = messages.find(
      (f: SelectedEmail) =>
        !notFolderOptions.includes(f.folderName!) && f.read === false
    );

    //如果移动到inbox或者sent,并且移动的邮件中有未读的邮件,则更新他们的邮件未读数
    if (
      (targetFolder === "inbox" || targetFolder === "sent") &&
      isHasUnreadMessage
    ) {
      loadAddress(targetFolder);
    }

    // 如果移动到文件夹,并且移动的邮件中有未读的邮件,则更新文件夹邮件未读数
    if (!notFolderOptions.includes(targetFolder) && isHasUnreadMessage) {
      loadFolders();
    }

    //如果移动的邮件中,有属于sent的未读邮件,并且移动的目标不是sent(防止重复调用),则更新sent邮件未读数
    if (isHasUnreadSentMessage && targetFolder !== "sent") {
      loadAddress("sent");
    }

    //如果移动的邮件中,有属于inbox的未读邮件,并且移动的目标不是inbox(防止重复调用),则更新inbox邮件未读数
    if (isHasUnreadInboxMessage && targetFolder !== "inbox") {
      loadAddress("inbox");
    }

    //如果移动的邮件中,有属于folder的未读邮件,并且移动的目标不是folder(防止重复调用),则更新folder邮件未读数
    if (isHasUnreadFolderMessage && notFolderOptions.includes(targetFolder)) {
      loadFolders();
    }
  };

  // 移动或者删除邮件后更新列表
  const reloadMessageList = async (
    emails: SelectedEmail[] | EmailMessage[]
  ) => {
    messageList.value = messageList.value.filter((f) => {
      return !emails.map((item) => item.id).includes(f.id);
    });

    if (!messageList.value.length) {
      await refreshMessageList();
    } else if (messageList.value.length < 30) {
      await reload();
    }
  };

  async function move(emails: any, folder: string) {
    await moveEmailApi(
      emails.map((item: any) => item.id),
      folder
    );
  }
  const deleteEmails = async (
    emails: SelectedEmail[] | EmailMessage[],
    folder: string
  ) => {
    if (folder === "trash") {
      await deleteEmailApi(emails.map((item) => item.id));
    } else if (folder === "search") {
      // 搜索列表的多选删除需要分开处理
      const tempEmails: any = emails;
      const trashEmails = tempEmails.filter(
        (f: any) => f.folderName === "trash"
      );
      const othersEmails = tempEmails.filter(
        (f: any) => f.folderName !== "trash"
      );

      if (trashEmails.length) {
        await deleteEmailApi(trashEmails.map((item: any) => item.id));
      }
      if (othersEmails.length) {
        await move(othersEmails as SelectedEmail[], "trash");
      }
    } else {
      await move(emails as SelectedEmail[], "trash");
    }
  };

  const getAttachmentType = (type: string) => {
    const imageTypes = ["gif", "jpg", "jpeg", "png", "webp", "svg+xml"];
    const zipTypes = ["zip", "x-rar-compressed"];
    const documentTypes = [
      "xlsx",
      "csv",
      "pdf",
      "css",
      "javascript",
      "html",
      "json",
      "vnd.openxmlformats-officedocument.wordprocessingml.document", //docx
      "vnd.openxmlformats-officedocument.presentationml.presentation", //ppts
      "vnd.ms-powerpoint", //ppt
      "msword", //doc
      "vnd.ms-excel", //xls
      "vnd.openxmlformats-officedocument.spreadsheetml.sheet", //xlsx
      "plain", //txt,
    ];
    const mediaTypes = ["mp4", "mpeg"];
    if (imageTypes.includes(type)) {
      return ImageIcon;
    } else if (zipTypes.includes(type)) {
      return ZipIcon;
    } else if (documentTypes.includes(type)) {
      return DocumentIcon;
    } else if (mediaTypes.includes(type)) {
      return MediaIcon;
    } else {
      return DefaultIcon;
    }
  };

  watch([address, folder], async (newValue, oldValue) => {
    //防止从邮件跳转到其他页面时，触发到watch中的loadMessage
    if (
      (route.matched[0].path !== "/kmail" &&
        route.matched[0].path !== "/kmail-settings") ||
      route.name === "calendar"
    )
      return;

    if (
      folder.value &&
      folder.value !== "compose" &&
      folder.value !== "addresses"
    ) {
      messageList.value = [];
      selectedMessage.value = [];

      if (
        (folder.value === "inbox" && isLoadAddress.value) ||
        (folder.value === "sent" && isLoadAddress.value)
      ) {
        await loadAddress(folder.value);
      }

      // 此时代表页面从邮箱设置那边跳转过来,主动更新folder子菜单列表(因为下面folderName存在时被return了)
      if (oldValue[1] === "addresses" && route.query.folderName) {
        await loadMessage();
        await loadFolders();
      }
      isLoadAddress.value = true;

      //不监听路由有folderName的情况，监听folderName动作一律交给下面的watch
      if (route.query.folderName) return;

      if (folder.value === "folder") {
        await loadFolders();
      }

      await loadMessage();
    }
  });

  watch(
    () => route.query.folderName,
    (newValue, oldValue) => {
      if (route.name === "compose") return;
      // 在邮件的一级菜单以及邮件一级菜单的详情页原地刷新时,则return
      if (!isLoadAddress.value && !route.query.folderName) return;
      // 如果是folder的二级菜单点击其他非文件夹菜单，则return
      if (route.name !== "folder" && oldValue && !newValue) {
        return;
      }
      messageKeyword.value = "";
      searchModel.value = null;
      refreshMessageList();
      loadFolders();
    },
    {
      immediate: true,
    }
  );

  watch(
    () => route.name,
    () => {
      if (route.query.flag) {
        if (route.query.flag === "advanced") {
          searchModel.value = {
            startDate: route.query?.startDate as string,
            endDate: route.query?.endDate as string,
            dateType: Number(route.query.dateType),
            searchFolder: route.query.searchFolder as string,
            from: route.query.from as string,
            keyword: route.query.keyword as string,
            position: route.query.position as string,
            readOrUnread: Number(route.query.readOrUnread),
            to: route.query.to as string,
          };
        } else {
          if (route.query.keyword) {
            messageKeyword.value = route.query.keyword;
          }
        }
      } else {
        searchModel.value = null;
        messageKeyword.value = "";
      }
      //保证在compose页面原地刷新时，再点击其他菜单能调用loadAddress()
      if (route.name === "compose") {
        isLoadAddress.value = true;
      }
      //保证从其他页面进入到邮箱时，不触发watch中的loadAddress()
      if (route.matched[0].path !== "/kmail") {
        // 离开邮箱页面时,重置关键词以及初始化isLoadAddress
        messageKeyword.value = "";
        searchModel.value = null;
        isLoadAddress.value = false;
      }
    },
    {
      immediate: true,
    }
  );

  return {
    domainList,
    addressList,
    folderList,
    sentAddressList,
    address,
    folder,
    sentUnreadCount,
    inboxUnreadCount,
    folderUnreadCount,
    messageList,
    loadDomains,
    loadAddress,
    loadFolders,
    loadSentAddressList,
    loadInboxAddressList,
    loadMessage,
    move,
    markRead,
    deleteEmails,
    refreshMessageList,
    reload,
    isShowNewMessageLogo,
    selectedMessage,
    messageKeyword,
    getAttachmentType,
    firstActiveMenu,
    searchModel,
    defaultAddress,
    preLatestId,
    folderMenuTree,
    activeName,
    searchEmailsCount,
    notFolderOptions,
    reloadMessageList,
    updateUnreadCount,
    justViewMessageId,
  };
});
