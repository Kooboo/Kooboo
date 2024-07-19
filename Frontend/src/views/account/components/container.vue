<script lang="ts" setup>
import LogoIcon from "@/assets/images/logo-transparent.svg?raw";
import { getQueryString } from "@/utils/url";
import { ref } from "vue";
import { useRouter } from "vue-router";
import LightSwitch from "@/components/light-switch/light-switch.vue";

import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";
defineProps<{ title: string; back?: boolean }>();
const { t } = useI18n();
const show = ref(false);
const router = useRouter();
const requireInfo = getQueryString("requireinfo");
const oauthId = getQueryString("oauthid");
const permission = getQueryString("permission");
const returnurl = getQueryString("returnurl");
const sso = getQueryString("sso");
const server = getQueryString("server");
const accessToken = getQueryString("access_token");

if (oauthId) {
  if (requireInfo?.toLowerCase() === "true") {
    router.replace({
      name: "bind-name",
      query: {
        id: oauthId,
        type: "oauth",
        permission,
        returnurl,
        sso,
        server,
      },
    });
  }
} else if (accessToken) {
  useAppStore().login(accessToken);
  router.replace({
    name: "home",
  });
} else {
  show.value = true;
}

const goLogin = () => {
  delete router.currentRoute.value.query.type;
  router.push({
    name: "login",
    query: {
      ...router.currentRoute.value.query,
    },
  });
};
</script>

<template>
  <div class="h-80px pl-40px flex items-center justify-between">
    <div
      class="dark:text-fff cursor-pointer"
      @click="goLogin"
      v-html="LogoIcon"
    />
    <LightSwitch class="border-none" />
  </div>
  <div v-if="show" class="w-296px mt-40px m-auto" v-bind="$attrs">
    <p
      class="text-40px h-40px relative font-bold mb-8 dark:text-fff/86"
      data-cy="title"
    >
      <span class="absolute whitespace-nowrap">
        {{ title }}
      </span>
    </p>
    <div
      v-if="back"
      class="flex items-center mb-24 text-444 dark:text-fff/86 text-m"
    >
      <div
        class="flex items-center cursor-pointer mr-4"
        data-cy="back"
        @click="$router.goBackOrTo({ name: 'login' })"
      >
        <el-icon class="iconfont icon-back mr-8" />
        <span class="mr-4">{{ t("common.back") }}</span>
      </div>
      <slot name="back-append" />
    </div>
    <slot />
  </div>
</template>
