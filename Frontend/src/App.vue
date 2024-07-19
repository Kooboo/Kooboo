<script lang="ts" setup>
import { ref, watch } from "vue";
import { useRoute } from "vue-router";
import { useSiteStore } from "./store/site";
import { getQueryString } from "./utils/url";
import Cookies from "universal-cookie";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { getPayload } from "@/utils/jwt";
import { useMonacoHoverWidgetOverflowFix } from "./hooks/use-monaco-fix";
import ForceUpgradeCover from "@/components/basic/force-upgrade-cover.vue";
import { useAppStore } from "./store/app";
import { vscodeLogin } from "./utils/common";
import "@/assets/fonts/font.css";

const cookies = new Cookies();
const { t } = useI18n();
const route = useRoute();
const siteStore = useSiteStore();
const appStore = useAppStore();
const currentSiteId = ref();
const mobileFlag = ref<any>(null);
handleAccessToken();
useMonacoHoverWidgetOverflowFix();

watch(
  () => route.query,
  () => {
    const siteId = getQueryString("siteId");
    if (!siteId) currentSiteId.value = undefined;
    if (route.meta.anonymous || !cookies.get("jwt_token")) return;

    if (siteId && siteId !== currentSiteId.value) {
      siteStore.loadSite();
      currentSiteId.value = siteId;
    }
  },
  { immediate: true }
);

//disable firefox drop open tab
document.body.ondrop = function (e) {
  e.preventDefault();
  e.stopPropagation();
};

function handleAccessToken() {
  const access_token = getQueryString("access_token");
  const auto_login = getQueryString("auto_login");
  const expires = new Date();
  expires.setDate(expires.getDate() + 30);

  if (access_token) {
    if (getQueryString("vscode-require-auth")) vscodeLogin();
    const payload = getPayload(access_token);
    if (
      location.pathname.indexOf("inline-design") > -1 ||
      payload?.redirect === location.host ||
      auto_login
    ) {
      cookies.set("jwt_token", access_token, { path: "/", expires });
      appStore.refreshLimitSite();
    }
  }
}
function isMobile() {
  mobileFlag.value = navigator.userAgent.match(
    /(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i
  );
  if (mobileFlag.value) {
    ElMessage.error(t("common.limitedFunctionalityOnMobile"));
  }
}

isMobile();
</script>

<template>
  <router-view />
  <ForceUpgradeCover v-if="appStore.showForceUpgrade" />
</template>

<style lang="scss">
html,
body,
#app {
  @apply h-full min-w-1140px overflow-hidden;
}

#app * :not(.monaco-editor *),
.el-popper * :not(.monaco-editor *),
.el-overlay * :not(.monaco-editor *),
.el-message-box * :not(.monaco-editor *) {
  @apply font-family;
}

// iconfont
.icon {
  width: 1em;
  height: 1em;
  vertical-align: -0.15em;
  fill: currentColor;
  overflow: hidden;
}

html {
  width: 100%;
}

body {
  line-height: 1;
  width: 100%;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: $main-color;
}

a {
  -webkit-backface-visibility: hidden;
  text-decoration: none;
  font-size: 14px;
}

/*下拉菜单的箭头旋转动画*/
[aria-expanded="true"] .icon-pull-down {
  transition: all 0.3s;
  transform: rotate(180deg);
}

[aria-expanded="false"] .icon-pull-down {
  transition: all 0.3s;
  transform: rotate(0);
}
</style>
