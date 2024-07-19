<template>
  <div
    class="language-list flex items-center justify-center cursor-pointer h-full border-l-1 border-r-1 border-[#e1e2e8] dark:border-opacity-50"
  >
    <el-dropdown trigger="click" class="h-full" @command="handleLanguageChange">
      <div class="flex items-center justify-center text-14px px-16 h-full">
        <img
          :src="appStore.currentLang === 'zh' ? cnImage : usImage"
          class="transform scale-60"
        />

        <el-icon class="iconfont icon-pull-down text-s leading-none" />
      </div>
      <template #dropdown>
        <el-dropdown-menu data-cy="lang-dropdown">
          <el-dropdown-item
            v-for="item in languages"
            :key="item.lang"
            :command="item.lang"
            :class="{ selected: appStore.currentLang === item.lang }"
            :disabled="appStore.currentLang === item.lang"
          >
            <div class="flex items-center space-x-8">
              <img
                :src="item.lang === 'zh' ? cnImage : usImage"
                class="w-22px h-22px"
              />
              <span class="language__item">{{ item.name }}</span>
            </div>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>

<script lang="ts" setup>
import { watch } from "vue";
import { useAppStore } from "@/store/app";
import { changeLanguage, languages, i18n } from "@/modules/i18n";
import { changeLanguage as changeLanguageApi } from "@/api/user";
import cnImage from "@/assets/images/🇨🇳@2x.png";
import usImage from "@/assets/images/🇺🇸@2x.png";

const appStore = useAppStore();

async function handleLanguageChange(lang: string) {
  if (lang !== appStore.currentLang && appStore.header) {
    await changeLanguageApi(lang);
    location.reload();
  }
}

watch(
  () => appStore.header,
  () => {
    if (appStore.header) {
      const lang = appStore.header.user.language;
      if ((i18n.global.locale as any).value != lang) {
        location.reload();
      }
    }
  },
  {
    deep: true,
    immediate: true,
  }
);
</script>
