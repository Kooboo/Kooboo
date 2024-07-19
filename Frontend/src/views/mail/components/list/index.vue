<template>
  <!-- 列表头部 -->
  <Header :title="title" :type="type" @refresh="refresh" />
  <!-- 操作列表 -->
  <div
    v-if="show"
    ref="list"
    class="shadow-s-10 rounded-normal overflow-hidden bg-fff px-32 dark:bg-[#333]"
  >
    <div
      class="h-64px text-m flex items-center justify-between bg-fff dark:bg-[#333]"
    >
      <div class="flex items-center space-x-12">
        <div
          class="rounded-full bg-[#F3F5F5] flex items-center h-30px px-16 dark:bg-444"
        >
          <el-checkbox
            :model-value="
              !!emailStore.selectedMessage.length &&
              emailStore.selectedMessage.length ===
                emailStore.messageList?.length
            "
            :indeterminate="
              !!emailStore.selectedMessage.length &&
              emailStore.selectedMessage.length !==
                emailStore.messageList?.length
            "
            data-cy="check-all"
            @change="onCheckAll"
          />
          <el-dropdown trigger="click">
            <span>
              <el-icon
                class="ml-12 iconfont icon-pull-down cursor-pointer text-black dark:text-[#ffffffaa]"
              />
            </span>

            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item data-cy="all" @click="onCheckType('all')">
                  <el-icon class="iconfont icon-classification" />
                  <span class="ml-8">{{ t("common.all") }}</span>
                </el-dropdown-item>

                <el-dropdown-item data-cy="read" @click="onCheckType('read')">
                  <el-icon class="iconfont icon-a-Emileopen" />
                  <span class="ml-8">{{ t("common.read") }}</span>
                </el-dropdown-item>

                <el-dropdown-item
                  data-cy="unread"
                  @click="onCheckType('unread')"
                >
                  <el-icon class="iconfont icon-a-Emilenotopen" />
                  <span class="ml-8">{{ t("common.unread") }}</span>
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
        <span
          class="rounded-full bg-[#F3F5F5] dark:bg-444 text-black dark:text-[#ffffffaa] flex items-center h-30px px-16 cursor-pointer mr-10px"
          :class="
            !emailStore.selectedMessage.length
              ? 'cursor-not-allowed !text-999 !text-opacity-60 dark:text-opacity-20'
              : ''
          "
          :tip="t('common.delete')"
          data-cy="delete"
          @click="deleteEmails(emailStore.selectedMessage.length)"
        >
          {{ t("common.delete") }}
        </span>
        <el-dropdown
          trigger="click"
          :disabled="!emailStore.selectedMessage.length ? true : false"
        >
          <div
            class="rounded-full bg-[#F3F5F5] flex items-center h-30px px-16 cursor-pointer dark:bg-444 text-black dark:text-[#ffffffaa]"
            :class="
              !emailStore.selectedMessage.length
                ? 'cursor-not-allowed text-999 text-opacity-60 dark:text-opacity-20'
                : ''
            "
            data-cy="setting"
          >
            <span>{{ t("common.markAs") }}</span>
            <el-icon class="ml-8 iconfont icon-pull-down" />
          </div>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item
                data-cy="mark-as-read"
                @click="changeSelectEmailsRead(true)"
              >
                <el-icon class="iconfont icon-a-Emileopen" />
                <span class="ml-8">{{ t("common.read") }}</span>
              </el-dropdown-item>

              <el-dropdown-item
                data-cy="mark-as-unread"
                @click="changeSelectEmailsRead(false)"
              >
                <el-icon class="iconfont icon-a-Emilenotopen" />
                <span class="ml-8">{{ t("common.unread") }}</span>
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
        <OperationDropdown
          v-if="type !== 'drafts'"
          :selected="emailStore.selectedMessage"
          @move-success="moveEmails"
        />
      </div>
      <span
        v-if="emailStore.selectedMessage.length"
        class="dark:text-999 text-[13px] ml-10px"
        >{{
          emailStore.selectedMessage.length > 1
            ? t("common.countEmailsSelected", {
                count: emailStore.selectedMessage.length,
              })
            : t("common.countEmailSelected", {
                count: emailStore.selectedMessage.length,
              })
        }}</span
      >
    </div>
    <div>
      <el-divider class="my-0" />
    </div>
    <!-- 邮件列表 -->
    <el-empty v-if="!emailStore.messageList.length" />

    <ul
      v-else
      ref="emailList"
      v-infinite-scroll="loadMore"
      :infinite-scroll-immediate="false"
      :infinite-scroll-distance="44"
      class="max-h-[calc(100vh-228px)] mb-16 !overflow-auto list-none"
    >
      <li
        v-for="item in emailStore.messageList"
        :id="item.id.toString()"
        :key="item.id"
        class="mail-item flex items-start mb-4 py-8 border-solid border-gray hover:bg-[#e8f4fd] dark:hover:bg-444 dark:bg-[#333]"
        :class="item.read ? '' : 'font-bold bg-[#f8f7f7] dark:bg-444'"
      >
        <div class="flex items-center h-28px">
          <el-icon
            class="iconfont icon-youjiantou text-blue"
            :class="
              route.query.messageId && emailStore.justViewMessageId === item.id
                ? ''
                : 'opacity-0'
            "
          />
          <span class="mr-16 flex items-center">
            <el-checkbox
              :model-value="
                emailStore.selectedMessage.some((s) => s.id === item.id)
              "
              data-cy="checkbox"
              @change="onCheck(item)"
            />
          </span>
          <el-tooltip
            placement="left"
            :content="
              !item.read ? t('common.markAsRead') : t('common.markAsUnread')
            "
          >
            <el-icon
              class="mr-8 iconfont font-normal cursor-pointer"
              :class="
                item.read
                  ? 'icon-a-Emileopen text-999/50'
                  : 'icon-a-Emilenotopen text-blue'
              "
              data-cy="mark-as-read-or-unread"
              @click="changeCurrentEmailStatus([item], !item.read)"
            />
          </el-tooltip>
        </div>

        <div
          class="ml-8 mr-32 flex-1 overflow-hidden cursor-pointer"
          @click="viewMessage(item)"
        >
          <div class="w-full flex overflow-hidden">
            <!-- name -->
            <span
              class="text-m w-168px ellipsis dark:text-[#fffa] mr-64 leading-7"
            >
              <span
                v-if="!['sent', 'drafts'].includes(item.folderName)"
                :title="item.from.name ?? item.from.address"
                data-cy="to-or-from"
              >
                {{ item.from.name ?? item.from.address }}
              </span>
              <span
                v-else-if="
                  !item.to.length && !(item.cc as unknown as EmailInfo[]).length && !(item.bcc as unknown as EmailInfo[]).length 
                "
              >
                {{ t("common.noRecipient") }}
              </span>

              <span
                v-else
                :title="[...item.to, ...item.cc!, ...item.bcc!].map((i) => i.name ?? i.address).join(', ')"
                data-cy="to-or-from"
              >
                {{ [...item.to, ...item.cc!, ...item.bcc!].map((i) => i.name ?? i.address).join(', ') }}
              </span>
            </span>
            <img
              v-if="item.hasAttachment"
              src="@/assets/images/attachment.svg"
              class="mr-8 h-7 w-16 dark:text-999"
            />
            <div v-else class="w-16px mr-8" />

            <!-- 主题 -->
            <div
              class="flex-1 text-m ellipsis max-w-600px dark:text-[#fffa] leading-7"
              data-cy="subject"
            >
              {{
                item.subject ? item.subject : "(" + t("common.noSubject") + ")"
              }}

              <!-- 附件 -->
              <div class="space-x-8 flex text-s">
                <div
                  v-for="itm in item.attachments.slice(0, 3)"
                  :key="itm.fileName"
                  class="border-[#A9ABB8] border-1 rounded-full font-normal flex items-center px-8 h-6"
                  :title="itm.fileName"
                  @click.stop="previewAttachment(itm.downloadUrl)"
                >
                  <img
                    :src="emailStore.getAttachmentType(itm.subType!.toLowerCase())"
                    class="mr-4 h-7 w-16"
                  />

                  <span class="max-w-120px ellipsis h-6 leading-6">{{
                    itm.fileName
                  }}</span>
                </div>
                <span
                  v-if="item.attachments.length > 3"
                  class="w-6 h-6 cursor-pointer ellipsis border-[#A9ABB8] border-1 rounded-full font-normal text-center leading-6 cursor-default"
                  @click.stop="viewMessage(item)"
                  >{{ item.attachments.length - 3 + "+" }}</span
                >
              </div>
            </div>
          </div>
        </div>

        <!-- 时间 -->
        <div
          class="mr-24 text-14px h-28px leading-7 flex items-center space-x-24 dark:text-[#fffa]"
        >
          <span
            v-if="route.name === 'searchEmail'"
            class="w-160px ellipsis text-green cursor-pointer"
            :title="getFolderName(item)"
            @click="navigateToThisFolder(item)"
            >{{ getFolderName(item) }}</span
          >
          <span class="w-150px" data-cy="date">{{ useTime(item.date) }}</span>
        </div>

        <!-- 投递状态 -->
        <div
          v-if="route.name !== 'searchEmail'"
          class="mr-24 text-m h-28px flex items-center dark:text-[#fffa]"
        >
          <el-icon
            v-if="item.deliveryLog !== null"
            :title="viewSendState(item.deliveryLog!, 'view')"
            class="iconfont cursor-pointer"
            :class="viewSendState(item.deliveryLog!, 'icon')"
            @click="showSentLog(item.deliveryLog!)"
          />
          <div v-else class="w-14px" />
        </div>
      </li>
    </ul>
  </div>

  <SentLogDialog
    v-if="showSentLogDialog"
    v-model="showSentLogDialog"
    :current-log="currentLog"
    :view-send-state="viewSendState"
  />
</template>

<script lang="ts" setup>
import type {
  DeliveryLog,
  EmailInfo,
  EmailItem,
  SelectedEmail,
} from "@/api/mail/types";
import { useTime } from "@/hooks/use-date";
import { useRouter, useRoute } from "vue-router";
import { ref, computed, onMounted, onUnmounted, nextTick, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useEmailStore } from "@/store/email";
import { useRouteEmailId } from "@/hooks/use-site-id";
import OperationDropdown from "../operation/index.vue";
import { getQueryString, openInNewTab } from "@/utils/url";
import { showDeleteMessageConfirm } from "@/components/basic/confirm";
import { ElMessage } from "element-plus";
import Header from "./header.vue";
import SentLogDialog from "@/views/mail/components/dialog/sent-log-dialog.vue";
import { showLoading, hideLoading } from "@/hooks/use-loading";

export interface Props {
  type: string;
}

const props = defineProps<Props>();
const router = useRouter();
const route = useRoute();
const showSentLogDialog = ref(false);
const currentLog = ref();

const showSentLog = (log: { isSuccess: boolean; items: DeliveryLog[] }) => {
  if (log) {
    showSentLogDialog.value = true;
    currentLog.value = log;
  }
};

watch(
  () => showSentLogDialog.value,
  () => {
    if (!showSentLogDialog.value) {
      currentLog.value = null;
    }
  }
);

const { t } = useI18n();
const emailStore = useEmailStore();
const selectType = ref();
const show = ref(false);
const previewAttachment = (url: string) => {
  openInNewTab(url);
};

const loadMore = async () => {
  await emailStore.loadMessage();
};

const refresh = async () => {
  await emailStore.refreshMessageList();
  if (route.name === "folder") {
    await emailStore.loadFolders();
  } else {
    await emailStore.loadAddress(emailStore.folder);
  }
  ElMessage.success(t("common.refreshSuccess"));
  if (!emailStore.messageList.length) return;
  document
    .getElementById(emailStore.messageList[0].id.toString())
    ?.scrollIntoView();
  emailStore.selectedMessage = [];
};

const FolderName: {
  inbox: string;
  sent: string;
  drafts: string;
  trash: string;
  spam: string;
  folder: string;
  [key: string]: string;
} = {
  inbox: t("common.inbox"),
  sent: t("mail.sent"),
  drafts: t("common.drafts"),
  trash: t("common.trash"),
  spam: t("common.spam"),
  folder: t("common.folders"),
};

const getFolderName = (message: EmailItem) => {
  const folderNamesArray = Object.keys(FolderName);

  let folderName = folderNamesArray.slice(0, 5).includes(message.folderName)
    ? FolderName[message.folderName]
    : message.folderName;
  let addressName = message.addressName;
  if (addressName) {
    return folderName + "/" + addressName;
  } else {
    return folderName;
  }
};

const navigateToThisFolder = (message: EmailItem) => {
  let folderName = message.folderName;
  let addressName = message.addressName;

  if (emailStore.notFolderOptions.includes(folderName)) {
    router.push({
      name: folderName,
      query:
        folderName === "inbox" || folderName === "sent"
          ? {
              address: addressName,
            }
          : {},
    });
  } else {
    router.push({
      name: "folder",
      query: {
        folderName: folderName,
      },
    });
  }
};

const title = computed(() => {
  // 如果是子文件夹菜单,则显示他的displayName
  return route.query.folderName
    ? route.query.folderName.toString().split("/")[
        route.query.folderName.toString().split("/").length - 1
      ]
    : FolderName[props.type];
});

const onCheckAll = (value: any) => {
  if (value) {
    emailStore.selectedMessage = emailStore.messageList.map((item) => {
      return Object.assign(
        {},
        { id: item.id, read: item.read, folderName: item.folderName }
      );
    });
  } else {
    emailStore.selectedMessage = [];
  }
};

const onCheckType = (type: "all" | "read" | "unread") => {
  if (type === "all") {
    emailStore.selectedMessage = emailStore.messageList.map((item) => {
      return Object.assign({}, { id: item.id, read: item.read });
    });
  } else if (type === "read") {
    emailStore.selectedMessage = emailStore.messageList
      .filter((item) => item.read)
      .map((item) => {
        return Object.assign({}, { id: item.id, read: item.read });
      });
  } else if (type === "unread") {
    emailStore.selectedMessage = emailStore.messageList
      .filter((item) => !item.read)
      .map((item) => {
        return Object.assign({}, { id: item.id, read: item.read });
      });
  }
  selectType.value = type;
};

const onCheck = (row: EmailItem) => {
  if (emailStore.selectedMessage?.find((f) => f.id === row.id)) {
    emailStore.selectedMessage = JSON.parse(
      JSON.stringify(emailStore.selectedMessage?.filter((f) => f.id !== row.id))
    );
  } else {
    emailStore.selectedMessage?.push({
      id: row.id,
      read: row.read,
      folderName: row.folderName,
    });
  }
};

const changeCurrentEmailStatus = (
  messages: SelectedEmail[] | EmailItem[],
  status: boolean
) => {
  changeEmailStatus(messages, status);
};

const changeSelectEmailsRead = (status: boolean) => {
  changeEmailStatus(emailStore.selectedMessage, status);
  emailStore.selectedMessage = [];
};

const changeEmailStatus = async (
  messages: SelectedEmail[] | EmailItem[],
  status: boolean
) => {
  await emailStore.markRead(
    messages.map((m) => m.id),
    status
  );
  const notFolderOptions = ["inbox", "sent", "drafts", "spam", "trash"];

  // 是否含有属于inbox的未读邮件
  const isHasUnreadInboxMessage = messages.find(
    (f: SelectedEmail | EmailItem) => f.folderName === "inbox"
  );

  // 是否含有属于sent的未读邮件
  const isHasUnreadSentMessage = messages.find(
    (f: SelectedEmail | EmailItem) => f.folderName === "sent"
  );

  // 是否含有属于folder的未读邮件
  const isHasUnreadFolderMessage = messages.find(
    (f: SelectedEmail | EmailItem) => !notFolderOptions.includes(f.folderName!)
  );

  //如果移动的邮件中,有属于sent的未读邮件,并且移动的目标不是sent(防止重复调用),则更新sent邮件未读数
  if (isHasUnreadSentMessage) {
    emailStore.loadAddress("sent");
  }

  //如果移动的邮件中,有属于inbox的未读邮件,并且移动的目标不是inbox(防止重复调用),则更新inbox邮件未读数
  if (isHasUnreadInboxMessage) {
    emailStore.loadAddress("inbox");
  }

  //如果移动的邮件中,有属于folder的未读邮件,并且移动的目标不是folder(防止重复调用),则更新folder邮件未读数
  if (isHasUnreadFolderMessage) {
    emailStore.loadFolders();
  }
};

const deleteEmails = async (count: number) => {
  if (!emailStore.selectedMessage.length) return;
  await showDeleteMessageConfirm(count);
  await emailStore.deleteEmails(emailStore.selectedMessage, props.type);

  showLoading();
  await emailStore.reload();
  hideLoading();
  await emailStore.updateUnreadCount(emailStore.selectedMessage, props.type);

  emailStore.selectedMessage = [];
};

const moveEmails = async (folder: string) => {
  await emailStore.move(emailStore.selectedMessage, folder);

  showLoading();
  await emailStore.reload();
  hideLoading();
  await emailStore.updateUnreadCount(emailStore.selectedMessage, folder);

  emailStore.selectedMessage = [];
};

// 查看邮件
const viewMessage = async (row: EmailItem) => {
  let folder = emailStore.folder;
  if (!row.read) {
    await changeEmailStatus([row], true);
  }
  if (row.id && route.query.address) {
    router.push(
      useRouteEmailId({
        name: "mail-content",
        query: {
          ...router.currentRoute.value.query,
          folder,
          address: route.query.address,
          messageId: row.id,
        },
      })
    );
  } else if (row.id && route.query.folderName) {
    router.push(
      useRouteEmailId({
        name: "mail-content",
        query: {
          ...router.currentRoute.value.query,
          folder,
          folderName: route.query.folderName,
          messageId: row.id,
        },
      })
    );
  } else {
    router.push(
      useRouteEmailId({
        name: "mail-content",
        query: {
          ...router.currentRoute.value.query,
          folder,
          messageId: row.id,
          activeMenu: emailStore.folder,
        },
      })
    );
  }
  emailStore.selectedMessage = [];
};

const viewSendState = (
  currentLog: {
    isSuccess: boolean;
    isSending: boolean;
    items: DeliveryLog[];
  },
  type: string
) => {
  let allItemsLength = currentLog?.items.length;
  let isSendingLength = currentLog?.items.filter(
    (f: DeliveryLog) => f.isSending === true
  ).length;
  let failItemsLength = currentLog?.items.filter(
    (f: DeliveryLog) => f.isSuccess === false
  ).length;
  let successItemsLength = currentLog?.items.filter(
    (f: DeliveryLog) => f.isSuccess === true
  ).length;

  switch (true) {
    case !isSendingLength && failItemsLength === allItemsLength:
      switch (type) {
        case "style":
          return "text-orange";
        case "view":
          return t("common.deliveryFailed");
        default:
          return "icon-fasongshibai text-orange";
      }
    case !isSendingLength && successItemsLength === allItemsLength:
      switch (type) {
        case "style":
          return "text-green";
        case "view":
          return t("common.deliverySuccessful");
        default:
          return "icon-fasongchenggong text-green";
      }
    case !isSendingLength && failItemsLength !== allItemsLength:
      switch (type) {
        case "style":
          return "text-[#d8bb4f]";
        case "view":
          return t("common.partialDeliverySuccessful");
        default:
          return "icon-bufenchenggong text-[#d8bb4f]";
      }
    default:
      switch (type) {
        case "style":
          return "text-999";
        case "view":
          return t("common.Delivering");
        default:
          return "icon-fasongzhong text-999";
      }
  }
};

onMounted(async () => {
  show.value = true;
  const messageId = getQueryString("messageId");
  await nextTick();
  if (messageId) {
    document.getElementById(messageId.toString())?.scrollIntoView();
  }
});

onUnmounted(() => {
  show.value = false;
});
</script>

<style lang="scss" scoped>
:deep(.el-checkbox__inner) {
  @apply w-16 h-16 border-[#979797];
}

// 设置多选框的第三状态样式
:deep(.el-checkbox__input.is-indeterminate .el-checkbox__inner::before) {
  top: 6px;
}

// 设置多选框的选中状态样式
:deep(.el-checkbox__inner::after) {
  top: 2px !important;
  left: 6px !important;
}

// // 滚动条样式*/
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

/* 滚动条上的滚动滑块 */
::-webkit-scrollbar-thumb {
  background-color: #dedfe1;
  border-radius: 4px;
}
.dark *::-webkit-scrollbar-thumb {
  background-color: #fff3;
}
</style>
