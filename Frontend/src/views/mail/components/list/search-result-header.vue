<template>
  <div class="text-999 text-14px h-16 leading-4">
    <div class="flex items-center">
      <span>{{ t("mail.contain") }}</span>
      <span class="ml-8">"</span>

      <span
        v-if="emailStore.searchModel"
        class="max-w-500px ellipsis"
        :title="getSearchResultText()"
      >
        {{ getSearchResultText() }}
      </span>
      <div
        v-else
        class="max-w-500px ellipsis"
        :title="route.query.keyword as string || ''"
      >
        {{ route.query.keyword }}
      </div>

      <span class="mr-8">"</span>
      <span>{{ t("mail.relatedEmails") }}:</span>
      <div v-if="emailStore.messageList.length">
        （{{
          emailStore.messageList.length > 1
            ? t("common.totalEmailsLoaded", {
                total: emailStore.messageList.length,
              })
            : t("common.totalEmailLoaded", {
                total: emailStore.messageList.length,
              })
        }}）
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import { useEmailStore } from "@/store/email";

const emailStore = useEmailStore();

const { t } = useI18n();
const route = useRoute();

const DateMap: {
  [x: number]: string;
  1: string;
  2: string;
  3: string;
  4: string;
  5: string;
} = {
  1: t("mail.inOneDay"),
  2: t("mail.inThreeDays"),
  3: t("mail.inAWeek"),
  4: t("mail.inTwoWeeks"),
  5: t("mail.inAMonth"),
};

const folderMap: {
  [x: string]: string;
  inbox: string;
  sent: string;
  draft: string;
  spam: string;
  trash: string;
} = {
  inbox: t("common.inbox"),
  sent: t("mail.sent"),
  draft: t("common.draft"),
  spam: t("common.spam"),
  trash: t("common.trash"),
};

const readOrUnreadMap: {
  [x: number]: string;
  0: string;
  1: string;
} = {
  0: t("common.unread"),
  1: t("common.read"),
};

const getFolderMap = (folder: string) => {
  return folderMap[folder] ?? folder;
};

const getSearchResultText = () => {
  if (!emailStore.searchModel) return;
  let result = "";
  let date;

  let keyword = emailStore.searchModel?.keyword;
  if (keyword) {
    result = result + keyword;
  }

  if (emailStore.searchModel?.dateType === 6) {
    date =
      emailStore.searchModel?.startDate !== emailStore.searchModel?.endDate
        ? emailStore.searchModel?.startDate +
          "~" +
          emailStore.searchModel?.endDate
        : emailStore.searchModel?.startDate;
  } else if (emailStore.searchModel?.dateType !== 0) {
    date = DateMap[emailStore.searchModel.dateType];
  }
  if (date) {
    if (emailStore.searchModel?.keyword) {
      date = ", " + date;
    }
    result = result + date;
  }

  let from = emailStore.searchModel?.from;
  if (from) {
    if (
      emailStore.searchModel?.keyword ||
      emailStore.searchModel?.dateType !== 0
    ) {
      from = ", " + from;
    }
    result = result + from;
  }

  let to = emailStore.searchModel?.to;
  if (to) {
    if (
      emailStore.searchModel?.keyword ||
      emailStore.searchModel?.dateType ||
      emailStore.searchModel?.from
    ) {
      to = ", " + to;
    }
    result = result + to;
  }

  let searchFolder =
    emailStore.searchModel?.searchFolder !== "allEmails"
      ? getFolderMap(emailStore.searchModel.searchFolder)
      : "";
  if (searchFolder) {
    if (
      emailStore.searchModel?.keyword ||
      emailStore.searchModel?.dateType ||
      emailStore.searchModel?.from ||
      emailStore.searchModel?.to
    ) {
      searchFolder = ", " + searchFolder;
    }
    result = result + searchFolder;
  }

  let readOrUnread =
    emailStore.searchModel?.readOrUnread !== -1
      ? readOrUnreadMap[emailStore.searchModel?.readOrUnread]
      : "";

  if (readOrUnread) {
    if (
      emailStore.searchModel?.keyword ||
      emailStore.searchModel?.dateType ||
      emailStore.searchModel?.from ||
      emailStore.searchModel?.to ||
      emailStore.searchModel?.searchFolder !== "allEmails"
    ) {
      readOrUnread = ", " + readOrUnread;
    }
    result = result + readOrUnread;
  }

  return result || t("common.allEmails");
};
</script>
