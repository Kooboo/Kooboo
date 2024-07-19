<script lang="ts" setup>
import { useSiteStore } from "@/store/site";
import { useRouter, useRoute } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const router = useRouter();
const route = useRoute();
var siteStore = useSiteStore();
</script>

<template>
  <div
    class="flex items-center px-5 pt-16 pb-12 border-b border-solid border-line dark:border-444 cursor-pointer space-x-12"
    data-cy="dashboard"
    @click="router.push(useRouteSiteId({ name: 'dashboard' }))"
  >
    <el-icon
      class="iconfont text-size-28px text-blue"
      :class="route.name === 'dashboard' ? 'icon-home2' : 'icon-home'"
    />
    <div class="transform scale-75 origin-left">
      <p
        class="text-2l ellipsis max-w-150px dark:text-fff/86"
        :title="siteStore.site.displayName"
      >
        {{ siteStore.site.displayName }}
      </p>
      <p class="text-999 dark:text-fff/60 text-s transform">
        {{ t("common.currentSite") }}
      </p>
    </div>
  </div>
</template>
