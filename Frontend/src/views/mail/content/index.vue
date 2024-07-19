<template>
  <div class="p-24 pb-16 flex">
    <div
      class="shadow-s-10 rounded-normal overflow-hidden bg-fff p-16 dark:bg-[#333] w-full transition-all duration-500"
      :class="
        getExtensionDrawerOpen ? 'box-border w-[calc(100%-22%-40px)]' : ''
      "
    >
      <div class="flex items-center text-m space-x-12 dark:text-[#fffa]">
        <template v-for="action in messageActions" :key="action.dataCy">
          <div
            v-if="action.showCondition"
            class="inline-flex rounded-full bg-[#F3F5F5] flex items-center h-30px px-16 cursor-pointer dark:bg-444"
            :data-cy="action.dataCy"
            @click="action.action"
          >
            <el-icon
              v-if="action.showBackIcon"
              class="mr-8 iconfont icon-back"
            />
            <span>{{ action.label }}</span>
          </div>
        </template>

        <OperationDropdown
          v-if="model.folderName !== 'drafts'"
          :selected="[{ id: model.id, read: true }]"
          :folder-name="model.folderName"
          @move-success="moveEmail"
        />
        <div class="flex-1" />
      </div>

      <el-divider class="my-8" />

      <el-scrollbar
        class="h-[calc(100vh-172px)] text-m"
        wrap-class="pr-12"
        view-class="h-full flex flex-col justify-between"
      >
        <div>
          <div class="p-16 bg-[#F3F5F5] dark:bg-444 dark:text-[#fffa]">
            <div
              class="mb-12 dark:text-[#fffa] flex items-center text-16px"
              style="word-break: break-word"
              data-cy="subject"
            >
              <img
                v-if="model.attachments?.length"
                src="@/assets/images/attachment.svg"
                class="mr-8 h-7 w-16 dark:text-999"
              />
              {{
                model.subject
                  ? model.subject
                  : "(" + t("common.noSubject") + ")"
              }}
            </div>

            <div
              v-if="
                !showDetails &&
                (model.to?.length || model.cc?.length || model.bcc?.length)
              "
              class="flex justify-between w-full"
            >
              <!-- 收件人信息 -->
              <div class="ellipsis max-w-7/10">
                <span :title="model.from?.name" class="max-w-1/2 text-666">
                  {{ model.from?.name ?? model.from?.address }}
                </span>
                <span class="mx-8">{{ t("content.to") }}</span>
                <span
                  class="text-666"
                  :title="
                    getAllRecipients([
                      ...JSON.parse(JSON.stringify(model.to || [])),
                      ...JSON.parse(JSON.stringify(model.cc || [])),
                      ...JSON.parse(JSON.stringify(model.bcc || [])),
                    ])
                  "
                  >{{
                    getAllRecipients([
                      ...JSON.parse(JSON.stringify(model.to || [])),
                      ...JSON.parse(JSON.stringify(model.cc || [])),
                      ...JSON.parse(JSON.stringify(model.bcc || [])),
                    ])
                  }}</span
                >
              </div>

              <!-- 时间以及详情开关 -->
              <div class="space-x-12">
                <span class="text-999">{{ useTime(model.date) }}</span>
                <span
                  class="text-blue cursor-pointer"
                  @click="showDetails = !showDetails"
                  >{{
                    showDetails ? t("common.hideDetails") : t("content.details")
                  }}</span
                >
              </div>
            </div>

            <div v-else class="max-w-full space-y-8">
              <div class="flex justify-between items-center">
                <!-- 发件人 -->
                <div class="flex items-center space-x-4 max-w-7/10">
                  <span class="dark:text-[#fffa]">{{ t("mail.from") }}:</span>
                  <div
                    class="ellipsis text-999 flex-1"
                    :title="model.from?.name + ' <' + model.from?.address + '>'"
                  >
                    <span data-cy="from">
                      <span class="text-666 mr-4">{{ model.from?.name }}</span>
                      <span> {{ `<${model.from?.address}>` }}</span>
                    </span>
                  </div>
                </div>

                <!-- 时间以及详情开关 -->
                <div
                  v-if="
                    model.to?.length || model.cc?.length || model.bcc?.length
                  "
                  class="space-x-12"
                >
                  <span class="text-999">{{ useTime(model.date) }}</span>
                  <span
                    class="text-blue cursor-pointer"
                    @click="showDetails = !showDetails"
                    >{{
                      showDetails
                        ? t("common.hideDetails")
                        : t("content.details")
                    }}</span
                  >
                </div>
              </div>
              <div v-if="model.to?.length" class="flex w-full">
                <span class="mr-4"> {{ t("mail.to") }}: </span>
                <div class="flex flex-1 flex-wrap items-center overflow-hidden">
                  <span
                    v-for="(recipient, index) in model.to"
                    :key="index"
                    class="text-999 flex max-w-full"
                  >
                    <div
                      class="ellipsis"
                      :title="recipient.name + ' <' + recipient.address + '>'"
                      data-cy="to"
                    >
                      <span class="text-666 mr-4">{{ recipient.name }}</span>
                      <span>
                        {{ `<${recipient.address}>` }}
                      </span>
                    </div>
                    <span class="mr-4">{{
                      index === model.to.length - 1 ? "" : ","
                    }}</span>
                  </span>
                </div>
              </div>

              <div v-if="model.cc?.length" class="ellipsis flex">
                <span class="mr-4"> {{ t("common.carbonCopy") }}: </span>
                <div class="flex flex-1 flex-wrap items-center overflow-hidden">
                  <span
                    v-for="(recipient, index) in model.cc"
                    :key="index"
                    class="text-999 flex max-w-full"
                  >
                    <div
                      class="ellipsis"
                      :title="recipient.name + ' <' + recipient.address + '>'"
                      data-cy="cc"
                    >
                      <span class="text-666 mr-4">{{ recipient.name }}</span>
                      <span>
                        {{ `<${recipient.address}>` }}
                      </span>
                    </div>
                    <span class="mr-4">{{
                      index === model.cc.length - 1 ? "" : ","
                    }}</span>
                  </span>
                </div>
              </div>

              <div
                v-if="
                  model.bcc?.length &&
                  (model.folderName === 'sent' || model.folderName === 'drafts')
                "
                class="flex"
              >
                <span class="mr-4"> {{ t("common.blindCarbonCopy") }}: </span>
                <div class="flex flex-1 flex-wrap items-center overflow-hidden">
                  <span
                    v-for="(recipient, index) in model.bcc"
                    :key="index"
                    class="text-999 flex max-w-full"
                  >
                    <div
                      class="ellipsis"
                      :title="recipient.name + ' <' + recipient.address + '>'"
                      data-cy="bcc"
                    >
                      <span class="text-666 mr-4"> {{ recipient.name }}</span>
                      <span>
                        {{ `<${recipient.address}>` }}
                      </span>
                    </div>
                    <span class="mr-4">{{
                      index === model.bcc.length - 1 ? "" : ","
                    }}</span>
                  </span>
                </div>
              </div>

              <!-- 附件 -->
              <div v-if="model.attachments?.length" class="flex mb-8 w-full">
                <div class="mr-4">{{ t("common.attachments") }}:</div>
                <div class="flex space-x-4 items-center text-m text-999">
                  <span
                    class="mr-4 whitespace-nowrap"
                    data-cy="total-attachments"
                    >{{
                      t("mail.quantity", { total: model.attachments.length })
                    }}</span
                  >
                  <span>(</span>
                  <div class="max-w-300px overflow-hidden flex items-center">
                    <div
                      class="ellipsis text-blue cursor-pointer max-w-full flex-shrink-0"
                      :title="model.attachments[0].fileName"
                      @click="attachmentScrollIntoView(true)"
                    >
                      {{ model.attachments[0].fileName }}
                    </div>
                    <span v-if="model.attachments.length > 1" class="text-blue"
                      >...</span
                    >
                  </div>
                  <span>)</span>
                </div>
              </div>

              <div class="flex items-center space-x-12">
                <button
                  class="text-blue"
                  data-cy="show-original-message"
                  @click="showOriginal"
                >
                  {{ t("common.showOriginalMessage") }}
                </button>
                <div class="h-14px w-1px bg-blue" />
                <button class="text-blue" @click="exportEmail">
                  {{ t("common.exportEmail") }}
                </button>
              </div>
            </div>
          </div>

          <!-- 会议邀请 -->
          <div
            v-if="model.calendar"
            class="p-16 bg-[#F3F5F5] dark:bg-444 dark:text-[#fffa] mt-8 flex items-center justify-between"
          >
            <div class="max-w-800px">
              <div>
                <span class="font-bold text-16px">{{
                  model.calendar[0].summary
                }}</span>
              </div>

              <div class="flex items-center mt-4 mb-16 font-bold text-16px">
                <div
                  :class="
                    useDate(model.calendar[0].start) ===
                    useDate(model.calendar[0].end)
                      ? 'flex space-x-8'
                      : ''
                  "
                >
                  <span>
                    {{ dayjs(model.calendar[0].start).year() }}
                    {{ useMonthDay(model.calendar[0].start) }} (
                    {{ week[new Date(model.calendar[0].start).getDay()] }} )
                  </span>
                  <span>
                    {{ useHourMinute(model.calendar[0].start) }}
                  </span>
                </div>
                <span class="mx-12">—</span>
                <div>
                  <span
                    v-if="
                      useDate(model.calendar[0].start) !==
                      useDate(model.calendar[0].end)
                    "
                  >
                    {{ useMonthDay(model.calendar[0].end) }}
                    ( {{ week[new Date(model.calendar[0].end).getDay()] }} )
                  </span>
                  <span>
                    {{ useHourMinute(model.calendar[0].end) }}
                  </span>
                </div>
              </div>
              <div class="my-8">
                <span v-if="model.calendar[0].location" class="mr-4 text-999"
                  >{{ t("common.location") }}:</span
                >{{ model.calendar[0].location }}
              </div>
              <div class="w-full flex">
                <div class="mr-4 text-999">{{ t("common.participants") }}:</div>
                <div class="flex-1 flex flex-wrap">
                  <span
                    v-for="(itm, idx) in model.calendar[0].attendees"
                    :key="itm"
                  >
                    {{ itm
                    }}<span
                      v-if="idx + 1 < (model.calendar[0].attendees.length || 0)"
                      >、</span
                    >
                  </span>
                </div>
              </div>
            </div>

            <div class="flex-shrink-0">
              <div v-if="!model.calendar[0].isExpired">
                <div
                  v-if="model.inviteConfirm === 0"
                  ref="invitationButtons"
                  class="flex items-center space-x-32"
                >
                  <div
                    class="flex flex-col items-center justify-center cursor-pointer hover:opacity-70 text-green"
                    @click="inviteDealing(1)"
                  >
                    <el-icon class="iconfont icon-yes text-28px mb-8" />
                    <div class="text-center">{{ t("content.yes") }}</div>
                  </div>

                  <div
                    class="flex flex-col items-center justify-center cursor-pointer hover:opacity-70 text-orange"
                    @click="inviteDealing(2)"
                  >
                    <el-icon class="iconfont icon-delete4 text-28px mb-8" />
                    <div class="text-center">{{ t("content.no") }}</div>
                  </div>

                  <div
                    class="flex flex-col items-center justify-center cursor-pointer hover:opacity-70 text-blue"
                    @click="inviteDealing(3)"
                  >
                    <el-icon class="iconfont icon-problem text-28px mb-8" />
                    <div class="text-center">{{ t("content.maybe") }}</div>
                  </div>
                </div>
                <div v-else>
                  <span
                    :class="{
                      'text-999': model.inviteConfirm === -1,
                      'text-green': model.inviteConfirm === 1,
                      'text-orange': model.inviteConfirm === 2,
                      'text-blue': model.inviteConfirm === 3,
                    }"
                    >{{ replyStatus(model.inviteConfirm) }}</span
                  >
                  <span
                    v-if="model.inviteConfirm !== -1"
                    class="ml-16 text-[#979797] cursor-pointer border-l-1 border-[#979797] px-16 hover:underline"
                    @click="reChoose"
                    >{{ t("common.reSelect") }}</span
                  >
                </div>
              </div>
              <div v-else class="text-999">
                {{ t("common.theInvitationHasExpired") }}
              </div>
            </div>
          </div>

          <RichShadow
            :html="model.html ?? ''"
            :hide-reply-calendar-button="model.calendar === null ? true : false"
            class="p-16 leading-6"
            :class="true ? 'aa' : ''"
          />
        </div>

        <!-- 附件 -->
        <div
          v-if="model.attachments?.length"
          ref="attachmentListRef"
          :class="{ 'active-background': activeAttachment }"
        >
          <AttachmentList
            :attachments="model.attachments || []"
            :download-all-attachment-path="model.downloadAttachment || ''"
          />
        </div>
      </el-scrollbar>
    </div>
    <ExtensionDrawer
      ref="extensionDrawerRef"
      :message-id="parseInt(getQueryString('messageId') ?? '0')"
      type="extendread"
      :height="'h-[calc(100vh-93px)]'"
    />
  </div>
</template>

<script lang="ts" setup>
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter, useRoute, onBeforeRouteLeave } from "vue-router";
import { getQueryString, openInNewTab } from "@/utils/url";
import type { EmailMessage } from "@/api/mail/types";
import { inviteDealingAPI, getContent } from "@/api/mail";
import { useEmailStore } from "@/store/email";
import ExtensionDrawer from "@/views/mail/components/extensions-drawer/index.vue";
import { useRouteEmailId } from "@/hooks/use-site-id";
import OperationDropdown from "../components/operation/index.vue";
import { showDeleteMessageConfirm } from "@/components/basic/confirm";
import { ElLoading } from "element-plus";
import AttachmentList from "./attachment-list.vue";

import {
  useDate,
  useTime,
  useMonthDay,
  useHourMinute,
  week,
} from "@/hooks/use-date";
import dayjs from "dayjs";
import RichShadow from "@/views/mail/components/rich-editor-content/index.vue";
import { showLoading, hideLoading } from "@/hooks/use-loading";

const router = useRouter();
const route = useRoute();

const { t } = useI18n();
const emailStore = useEmailStore();
const model = ref({} as EmailMessage);
const extensionDrawerRef = ref();
const getExtensionDrawerOpen = computed(() => {
  return extensionDrawerRef.value?.open ?? false;
});

const pageLen = ref(0);
const currentIndex = ref(0);
const invitationButtons = ref<HTMLElement>();
const showDetails = ref(false);
const attachmentListRef = ref();
const activeAttachment = ref(false);
const activeAttachmentFlag = ref(false);

const replyStatus = (status: number) => {
  const statusMessages: Record<string, string> = {
    "-1": t("common.theInvitationHasExpired"),
    "1": t("common.acceptedTheInvitation"),
    "2": t("common.declinedTheInvitation"),
    default: t("common.tentativelyAcceptedTheInvitation"),
  };

  return statusMessages[status.toString()] || statusMessages.default;
};

const attachmentScrollIntoView = () => {
  activeAttachmentFlag.value = true;

  // 附件滚动到可视范围内再激活背景色
  if (activeAttachmentFlag.value) {
    attachmentListRef.value.scrollIntoView({ behavior: "smooth" });
    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          activeAttachment.value = true;
          setTimeout(() => {
            activeAttachment.value = false;
          }, 500);
          observer.disconnect();
          activeAttachmentFlag.value = false;
        }
      });
    });
    observer.observe(attachmentListRef.value);
  }
};

function getAllRecipients(data: { name: string; address: string }[]) {
  let result = "";

  for (let i = 0; i < data.length; i++) {
    const item = data[i];
    if (item.name.trim() !== "") {
      result += item.name;
      if (i < data.length - 1) {
        result += ", ";
      }
    } else {
      result += item.address;
      if (i < data.length - 1) {
        result += ", ";
      }
    }
  }

  return result;
}
// 搜索邮件的路由参数，用于从邮件详情返回列表页或者下一页时，保留搜索参数
let searchQuery = route.query.flag
  ? route.query.flag === "common"
    ? {
        flag: "common",
        keyword: emailStore.messageKeyword,
      }
    : {
        flag: "advanced",
        ...emailStore.searchModel,
      }
  : undefined;

const moveEmail = async (folder: string) => {
  await emailStore.move([model.value], folder);

  showLoading();
  await emailStore.reload();
  hideLoading();
  await emailStore.updateUnreadCount(emailStore.selectedMessage, folder);

  if (route.query.folder === "searchEmail") {
    await nextOrBackList(model.value.folderName !== "trash" ? true : false);
  } else {
    await nextOrBackList();
  }
};

const deleteEmailToTrash = async (message: EmailMessage[]) => {
  await showDeleteMessageConfirm(1);
  await emailStore.deleteEmails(message, model.value.folderName);

  showLoading();
  await emailStore.reload();
  hideLoading();
  await emailStore.updateUnreadCount(emailStore.selectedMessage, "trash");

  /* 
    从搜索列表进入的详情页,如果当前操作的邮件不是trash文件夹的,这时操作完后
    会更新列表而搜索列表这边删除非trash邮件时,该邮件仍存在于邮件列表中，所以
    需要手动改变currentIndex来跳到下一封邮件。
  */
  if (route.query.folder === "searchEmail") {
    await nextOrBackList(model.value.folderName !== "trash" ? true : false);
  } else {
    await nextOrBackList();
  }
};
const messageActions = computed(() => {
  return [
    {
      showCondition: true,
      dataCy: "back",
      action: () => backToList(false),
      showBackIcon: true,
      label: t("common.back"),
    },
    {
      showCondition:
        model.value.folderName === "sent" ||
        model.value.folderName === "drafts",
      dataCy: "editAgain",
      action: edit,
      showBackIcon: false,
      label: t("common.editAgain"),
    },
    {
      showCondition: model.value.folderName !== "drafts",
      dataCy: "reply",
      action: () => reply(false),
      showBackIcon: false,
      label: t("common.reply"),
    },
    {
      showCondition: model.value.folderName !== "drafts",
      dataCy: "reply-all",
      action: () => reply(true),
      showBackIcon: false,
      label: t("common.replyAll"),
    },
    {
      showCondition: model.value.folderName !== "drafts",
      dataCy: "forward",
      action: forward,
      showBackIcon: false,
      label: t("common.forward"),
    },
    {
      showCondition: true,
      dataCy: "delete",
      action: () => deleteEmailToTrash([model.value]),
      showBackIcon: false,
      label: t("common.delete"),
    },
  ];
});

const showOriginal = () => {
  let originalRoute = router.resolve({
    name: "original message",
    query: {
      messageId: route.query.messageId,
    },
  });
  openInNewTab(originalRoute.href);
};

const exportEmail = () => {
  openInNewTab(
    `/_api/EmailMessage/ExportEmlFile/${model.value.id}?subject=${model.value.subject}`
  );
};

const inviteDealing = async (inviteConfirm: number) => {
  await inviteDealingAPI(
    model.value.id,
    inviteConfirm,
    model.value.calendar,
    model.value.to[0].address
  );
  model.value.inviteConfirm = inviteConfirm;
};

const reChoose = () => {
  model.value.inviteConfirm = 0;
};

async function load(messageId?: number | string) {
  const loadingInstance = ElLoading.service({
    background: "rgba(0, 0, 0, 0.5)",
  });

  if (!messageId) {
    messageId = getQueryString("messageId");
  }
  try {
    model.value = await getContent(messageId!);

    // 去掉给签名加上的read-only属性
    model.value.html = model.value.html.replace(
      /<div class="signature"([^>]*)style=["'][^"']*["']([^>]*)>/gi,
      '<div class="signature"$1$2>'
    );

    // 若a标签没有target="_blank"属性，则加上
    model.value.html = model.value.html.replace(
      /<a\s+([^>]*)>/gi,
      (match, p1) => {
        if (!p1.includes('target="_blank"')) {
          return `<a ${p1} target="_blank">`;
        } else {
          return match;
        }
      }
    );
  } catch {
    backToList(true);
  }

  pageLen.value = emailStore.messageList?.length || 0;
  const index = emailStore.messageList?.findIndex(
    (item) => item.id === model.value.id
  );

  if (index !== undefined && index >= 0) {
    currentIndex.value = index;
  } else {
    currentIndex.value = 0;
  }

  nextTick(async () => {
    loadingInstance.close();
  });
}

async function nextOrBackList(skipThisEmail?: boolean) {
  pageLen.value = emailStore.messageList.length;

  // 搜索列表进入详情的情况下:
  if (route.query.folder === "searchEmail") {
    let isBack = false;
    if (
      (currentIndex.value >= pageLen.value &&
        model.value.folderName === "trash") ||
      (currentIndex.value >= pageLen.value - 1 &&
        model.value.folderName !== "trash")
    ) {
      isBack = true;
    }
    // 邮件列表的长度小于30并且当前操作的邮件是最后一封时
    if (pageLen.value !== 30 && isBack) {
      backToList(currentIndex.value > 0 ? true : false);
      return;
    }

    // 邮件列表的长度小于搜索结果的长度，并且只获取了30条邮件的情况下，
    // 如果当前的邮件是第30封，需要再获取下一页的邮件来跳转到下一封邮件。
    if (
      pageLen.value === 30 &&
      currentIndex.value === 29 &&
      pageLen.value < emailStore.messageList.length
    ) {
      await emailStore.loadMessage();
    }
  }

  if (currentIndex.value < pageLen.value) {
    // skipThisEmail为true时代表需要手动改变currentIndex来跳到下一封邮件
    let id = skipThisEmail
      ? emailStore.messageList?.[currentIndex.value + 1].id
      : emailStore.messageList?.[currentIndex.value].id;
    router.replace({
      name: route.name as string,
      query: {
        folder: route.query.folder,
        activeMenu: route.query.activeMenu,
        folderName: route.query.folderName,
        address: route.query.folderName ? undefined : route.query.address,
        messageId: id,
        ...searchQuery,
      },
    });
    if (id) {
      await load(id);
      // 如果下一封是未读的,更新未读数
      emailStore.messageList.forEach(async (f) => {
        if (f.id === id && !f.read) {
          f.read = true;
          if (route.query.folder === "folder") {
            await emailStore.loadFolders();
          } else {
            await emailStore.loadAddress(route.query.folder as string);
          }
        }
      });
    }
  } else {
    // 没有下一封则返回列表
    backToList(currentIndex.value > 0 ? true : false);
  }
}

// 为了点击浏览器后退时能够定位到当前详情页的锚点
onBeforeRouteLeave(async (to, from) => {
  const folderType = [
    "inbox",
    "sent",
    "drafts",
    "spam",
    "trash",
    "searchEmail",
    "folder",
  ];
  if (folderType.includes(to.name as string)) {
    if (to.query.messageId !== route.query.messageId) {
      return {
        name: to.name,
        replace: true,
        query: { ...to.query, messageId: route.query.messageId },
      } as any;
    }
  }
  // 记录当前查看的邮件id,为了返回列表的时候能够标记出来
  emailStore.justViewMessageId = Number(route.query.messageId);
});

load();

async function backToList(isLoadMessage: boolean) {
  await router.replace({
    name: route.query.folder as string,
    query: {
      address: route.query.address ?? "",
      folderName: route.query.folderName ?? undefined,
      messageId: isLoadMessage
        ? emailStore.messageList[currentIndex.value - 1].id
        : route.query.messageId,
      ...searchQuery,
    },
  });
}

//编辑
function edit() {
  if (model.value.id) {
    router.push(
      useRouteEmailId({
        name: "compose",
        query: {
          type: model.value.folderName === "sent" ? "EditAgain" : "Drafts",
          sourceId: model.value.id,
          ...router.currentRoute.value.query,
          oldActiveMenu: route.query.folder,
          folder: "compose",
        },
      })
    );
  }
}

// 回复
function reply(isReplyAll: boolean) {
  if (model.value.id) {
    // 替换掉路由中的folderName参数名,用于compose返回时能够进入到folderName的watch
    let oldFolderName = route.query.folderName;
    if (route.query.folderName) {
      delete router.currentRoute.value.query.folderName;
    }
    router.push(
      useRouteEmailId({
        name: "compose",
        query: {
          type: isReplyAll ? "ReplyAll" : "Reply",
          sourceId: model.value.id,
          ...router.currentRoute.value.query,
          //把之前激活的左侧菜单存储起来
          oldActiveMenu: route.query.folder,
          oldFolderName: oldFolderName ?? undefined,
          //改变回复页面激活的左侧菜单
          folder: "compose",
        },
      })
    );
  }
}

//转发
function forward() {
  if (model.value.id) {
    // 替换掉路由中的folderName参数名,用于compose返回时能够进入到folderName的watch
    let oldFolderName = route.query.folderName;
    if (route.query.folderName) {
      delete router.currentRoute.value.query.folderName;
    }
    router.push(
      useRouteEmailId({
        name: "compose",
        query: {
          type: "Forward",
          sourceId: model.value.id,
          ...router.currentRoute.value.query,
          oldActiveMenu: route.query.folder,
          oldFolderName: oldFolderName ?? undefined,
          folder: "compose",
        },
      })
    );
  }
}
</script>

<style>
:root {
  --active-bg: #e8f4fe;
}

.dark {
  --active-bg: #313d47;
}
</style>

<style scope>
.active-background {
  animation: active 0.5s linear 1;
}

@keyframes active {
  0%,
  100% {
    background-color: inherit;
  }
  50% {
    background-color: var(--active-bg);
  }
}
</style>
