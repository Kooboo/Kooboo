<script setup lang="ts">
import { LatestMsgId } from "@/api/mail";
import { useEmailStore } from "@/store/email";
import { computed, onMounted, onUnmounted, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter, useRoute } from "vue-router";

const router = useRouter();
const route = useRoute();

const { t } = useI18n();
const emailStore = useEmailStore();
const isShow = computed(() => {
  return (
    emailStore.isShowNewMessageLogo === "true" &&
    route.matched[0].path !== "/kmail"
  );
});
onMounted(async () => {
  if (route.matched[0].path !== "/kmail") {
    emailStore.preLatestId = await LatestMsgId();
  }
});

//用于判断是否有新邮件进来的定时器
let timer: ReturnType<typeof setInterval> | null = null;
clearInterval(timer!);
timer = setInterval(async () => {
  if (route.matched[0].path === "/kmail") return;
  let latestId = await LatestMsgId();
  if (latestId > emailStore.preLatestId) {
    emailStore.isShowNewMessageLogo = "true";
    emailStore.preLatestId = latestId;
  }
}, 300000);
onUnmounted(() => {
  clearInterval(timer!);
});
watch(
  () => emailStore.isShowNewMessageLogo,
  (n) => {
    if (n === "true") {
      localStorage.setItem("isHasNewMessage", "true");
    } else {
      localStorage.setItem("isHasNewMessage", "false");
    }
  }
);
</script>

<template>
  <div
    class="relative px-16 border-l-1 border-[#e1e2e8] dark:border-opacity-50 h-full flex items-center justify-center cursor-pointer group text-m hover:text-blue"
    data-cy="mail"
    @click="
      router.push({
        name: 'inbox',
      })
    "
  >
    <span>{{ t("common.mail") }}</span>
    <div
      class="rounded-full w-8 h-8 bg-[#e5574c] left-[46px] top-12"
      :class="isShow ? 'absolute' : 'hidden'"
    />
  </div>
</template>
