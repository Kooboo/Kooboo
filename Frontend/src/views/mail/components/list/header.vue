<template>
  <div class="mb-16 flex items-center justify-between dark:text-[#fffa] text-s">
    <div>
      <SearchResultHeader v-if="route.name === 'searchEmail'" />
      <div v-else>
        <span class="text-l">{{ title }}</span>
        <span v-if="getUnreadCount">
          （
          {{
            t("common.unreadEmail", {
              unread: getUnreadCount,
            })
          }}
          ）
        </span>

        <el-tooltip
          v-if="route.name !== 'searchEmail'"
          effect="dark"
          :content="t('common.refresh')"
          placement="top"
        >
          <el-button
            class="!w-28px !h-28px ml-8"
            circle
            data-cy="refresh"
            @click="emits('refresh')"
          >
            <el-icon class="iconfont icon-Refresh text-blue" />
          </el-button>
        </el-tooltip>
      </div>
    </div>

    <EmailSearch />
  </div>
</template>
<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { computed } from "vue";

import { useEmailStore } from "@/store/email";
import EmailSearch from "@/views/mail/components/email-search/index.vue";
import SearchResultHeader from "@/views/mail/components/list/search-result-header.vue";

const { t } = useI18n();
const route = useRoute();
const emailStore = useEmailStore();
const props = defineProps<{
  title?: string;
  type: string;
}>();
const emits = defineEmits<{
  (e: "refresh"): void;
}>();

const getUnreadCount = computed(() => {
  if (route.query?.address) {
    if (props.type === "inbox") {
      return emailStore.addressList.filter(
        (i) => i.address === route.query.address
      )[0]?.unRead;
    } else if (props.type === "sent") {
      return emailStore.sentAddressList.filter(
        (i) => i.address === route.query.address
      )[0]?.unRead;
    }
  }
  return getUnreadCountBasedOnType();
});

const getUnreadCountBasedOnType = () => {
  if (props.type === "inbox") {
    return emailStore.inboxUnreadCount;
  } else if (props.type === "sent") {
    return emailStore.sentUnreadCount;
  } else if (props.type === "folder") {
    if (route.query.folderName) {
      return emailStore.folderList?.filter(
        (i) => i.name === route.query.folderName
      )[0]?.unRead;
    } else {
      return emailStore.folderUnreadCount;
    }
  }
  return 0;
};
</script>
