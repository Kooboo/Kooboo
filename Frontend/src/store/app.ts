import { defineStore } from "pinia";
import { getHeader } from "@/api/bar";
import type { Header } from "@/api/bar/types";
import { i18n } from "@/modules/i18n";
import { computed, ref, unref } from "vue";
import { languages } from "@/modules/i18n";
import { usePersistent } from "./persistent";
import { getPayload } from "@/utils/jwt";
import Cookies from "universal-cookie";
import { useSiteStore } from "./site";
import { getQueryString } from "@/utils/url";
import type { Organization } from "@/api/organization/types";
import { getCurrentDataCenter, getOrg } from "@/api/organization";
import { vscodeLogin } from "@/utils/common";
const cookies = new Cookies();

export const useAppStore = defineStore("appStore", () => {
  const header = ref<Header>();
  const { token, isOnlineServer } = usePersistent();
  const currentDataCenter = ref();
  const currentOrg = ref<Organization>();
  const showForceUpgrade = ref(false);
  const limitSite = ref("");

  function refreshLimitSite() {
    const token = cookies.get("jwt_token");
    if (!token) return;
    const payload = getPayload(token);
    if (payload?.kind == "bearer") {
      limitSite.value = payload.siteId as any;
    } else {
      limitSite.value = "";
    }
  }

  refreshLimitSite();

  const load = async () => {
    if (!header.value) {
      currentOrg.value = await getOrg();
      header.value = await getHeader();
      if (currentOrg.value.isAdmin) {
        currentDataCenter.value = await getCurrentDataCenter();
      }
    }
  };

  const currentLang = computed(() => {
    return header.value?.user?.language || unref(i18n.global.locale);
  });

  const locale = computed(() => {
    const item = languages.value.find((f) => f.lang === currentLang.value);
    return item?.elementPackage;
  });

  const logout = () => {
    cookies.remove("jwt_token", { path: "/" });
    header.value = undefined;
    token.value = null;
    if (isOnlineServer.value) {
      let domain = "kooboo.com";
      if (location.hostname.endsWith("kooboo.cn")) {
        domain = "kooboo.cn";
      }
      location.href = `https://www.${domain}/_start/_logout`;
      throw Error("Need redirect");
    }
  };

  const login = (accessToken: string) => {
    useSiteStore().clear();
    if (getQueryString("vscode-require-auth")) vscodeLogin(accessToken);
    const payload = getPayload(accessToken);
    let domain = payload.redirect;
    if (typeof domain === "string") domain = domain?.toLowerCase();
    const returnUrl = getQueryString("returnurl");
    if (
      domain &&
      domain !== location.host &&
      isOnlineServer.value &&
      !getQueryString("permission")
    ) {
      let startPath = "_start";
      if (typeof domain === "string") {
        if (
          domain.indexOf("ninjible.com") > -1 ||
          domain.indexOf("islandguide") > -1
        ) {
          startPath = "_Admin/login";
        }
      }

      let url = `https://${domain}/${startPath}?access_token=${accessToken}`;
      const expires = new Date();
      expires.setDate(expires.getDate() + 30);
      cookies.set("kooboo_redirect_url", url, {
        path: "/",
        expires: expires,
      });
      if (returnUrl) url += `&returnurl=${returnUrl}`;
      location.href = url;
      throw new Error("Need redirect");
    }

    token.value = accessToken;
    const expires = new Date();
    expires.setDate(expires.getDate() + 30);
    cookies.set("jwt_token", accessToken, {
      path: "/",
      expires: expires,
    });
    refreshLimitSite();

    if (getQueryString("sso") == "1") {
      const server = getQueryString("server");
      location.href = `${server}/_api/v2/user/ssoLogin?accessToken=${accessToken}&returnUrl=${returnUrl}`;
      throw new Error("Need redirect");
    }

    if (returnUrl) {
      location.href = returnUrl;
      throw new Error("Need redirect");
    } else {
      load();
    }
  };

  function forceUpgrade() {
    showForceUpgrade.value = true;
  }

  return {
    header,
    currentLang,
    locale,
    currentDataCenter,
    currentOrg,
    logout,
    login,
    load,
    forceUpgrade,
    showForceUpgrade,
    limitSite,
    refreshLimitSite,
  };
});
