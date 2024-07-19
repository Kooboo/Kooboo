<script lang="ts" setup>
import { useRouter } from "vue-router";
import { getUrl } from "@/api/oauth";
import { dark } from "@/composables/dark";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const router = useRouter();

const loginMethods = [
  {
    name: "phone",
    display: t("common.phone"),
    icon: "icon-phone",
    color: "#2296F3",
  },
  {
    name: "wechat",
    display: t("common.Wechat"),
    icon: "icon-WeChat",
    color: "#1AAD19",
  },
  {
    name: "google",
    display: "Google",
    icon: "icon-Google",
    color: "#1A73E8",
  },
  {
    name: "github",
    display: "Github",
    icon: "icon-github",
    color: "#000000",
    darkColor: "#fff",
  },
];

const auth = (name: string) => {
  if (name === "phone") {
    router.push({
      name: `${name}-login`,
      query: { ...router.currentRoute.value.query },
    });
  } else {
    getUrl(location.href, name).then((r) => {
      location.href = r;
    });
  }
};
</script>

<template>
  <div class="flex items-center justify-center">
    <div
      v-for="item of loginMethods"
      :key="item.name"
      class="w-88px h-88px text-center rounded-normal cursor-pointer flex items-center justify-center flex-col hover:bg-blue/20"
      type="text"
      @click="auth(item.name)"
    >
      <el-icon
        :class="[item.icon, dark && item.darkColor]"
        :alt="item.display"
        class="iconfont text-40px rounded-full"
        :style="{ color: dark ? item.darkColor ?? item.color : item.color }"
      />
      <p class="text-999 mt-4 text-s">{{ item.display }}</p>
    </div>
  </div>
</template>

<style scoped>
div:hover > p {
  color: black;
}
.dark div:hover > p {
  color: initial;
}
</style>
