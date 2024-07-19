<script lang="ts" setup>
import LanguageList from "./language-list.vue";
import Profile from "./profile.vue";
import LogoIcon from "@/assets/images/logo-transparent.svg?raw";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";

const { t } = useI18n();

const router = useRouter();

const appStore = useAppStore();
</script>
<template>
  <nav
    class="flex items-center bg-fff dark:(bg-[#333] text-[#fffa]) z-100 h-48px relative shadow-s-10"
  >
    <div
      v-if="appStore.limitSite"
      class="flex items-center cursor-pointer w-162px"
      v-html="LogoIcon"
    />
    <!-- eslint-disable-next-line vue/no-v-html -->
    <router-link v-else :to="{ name: 'home' }" data-cy="logo">
      <div class="flex items-center cursor-pointer w-162px" v-html="LogoIcon" />
    </router-link>

    <div class="flex-1 flex items-center h-full">
      <slot name="left" />
      <a
        v-if="!appStore.limitSite"
        class="h-full !mx-0 px-16px flex items-center justify-center cursor-pointer hover:text-blue border-l-1 border-[#e1e2e8] dark:border-opacity-50"
        @click="
          router.push({
            name: 'templates',
          })
        "
      >
        {{ t("common.market") }}
      </a>
      <LanguageList
        v-if="!appStore.limitSite"
        class="hover:bg-[#EFF6FF] dark:hover:bg-444"
      />

      <LightSwitch v-if="!appStore.limitSite" />

      <div class="flex-1" />
      <slot name="center" />
      <div class="flex-1" />
      <slot name="right" />

      <a
        v-if="
          appStore.currentOrg?.isPartner &&
          appStore.currentOrg?.isAdmin &&
          !appStore.limitSite
        "
        class="h-full !mx-0 px-16px flex items-center justify-center cursor-pointer hover:text-blue border-l-1 border-[#e1e2e8] dark:border-opacity-50"
        data-cy="console"
        @click="
          router.push({
            name: 'partner',
          })
        "
      >
        {{ t("common.partner") }}
      </a>
      <a
        v-if="!appStore.limitSite"
        class="h-full !mx-0 px-16px flex items-center justify-center cursor-pointer hover:text-blue border-l-1 border-r-1 border-[#e1e2e8] dark:border-opacity-50"
        data-cy="console"
        @click="
          router.push({
            name: 'kconsole',
          })
        "
      >
        {{ t("common.console") }}
      </a>
      <div
        v-if="appStore.currentOrg?.isAdmin && !appStore.limitSite"
        class="px-16 border-r-1 border-[#e1e2e8] dark:border-opacity-50 h-full flex items-center justify-center group cursor-pointer"
        @click="
          router.push({
            name: 'serviceDataCenter',
          })
        "
      >
        <span
          class="group-hover:text-blue text-m"
          data-cy="current-data-center"
          >{{ appStore.currentDataCenter }}</span
        >
      </div>

      <Profile class="hover:bg-[#EFF6FF] dark:hover:bg-444" />
    </div>
  </nav>
</template>
