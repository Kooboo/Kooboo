<script lang="ts" setup>
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import RightNav from "./components/right-nav.vue";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";
import { computed } from "vue";
const { t } = useI18n();

const crumbPath = computed(() => {
  return [
    {
      name: t("common.allSites"),
      route: {
        name: "home",
      },
    },
    ...(getQueryString("currentFolder")
      ? [
          {
            name: getQueryString("currentFolder")!,
            route: {
              name: "home",
              query: {
                currentFolder: getQueryString("currentFolder"),
              },
            },
          },
        ]
      : []),
    {
      name: t("common.newSite"),
    },
  ];
});
</script>

<template>
  <div class="w-1120px mx-auto py-32">
    <Breadcrumb hide-header :crumb-path="crumbPath" />
    <div class="flex mt-32">
      <div
        class="flex-1 mr-32 rounded-normal bg-fff dark:bg-[#333] shadow-s-10 p-24"
      >
        <router-view />
      </div>
      <div>
        <div class="w-364px bg-card dark:bg-444 rounded-normal shadow-s-10">
          <RightNav />
        </div>
      </div>
    </div>
  </div>
</template>
