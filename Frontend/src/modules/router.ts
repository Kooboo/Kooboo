import type { Install } from "./types";
import type { RouteLocationRaw } from "vue-router";
import { createRouter, createWebHistory } from "vue-router";
import routes from "@/router/routes";
import { ElLoading } from "element-plus";
import Cookies from "universal-cookie";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useSiteStore } from "@/store/site";
import { countRecentVisits } from "@/global/recent-visits";
const cookies = new Cookies();

let loadingInstance: { close: () => void } | undefined;

function showLoading() {
  if (!loadingInstance) {
    loadingInstance = ElLoading.service({ background: "rgba(0, 0, 0, 0.5)" });
  }
}

function hideLoading() {
  loadingInstance?.close();
  loadingInstance = undefined;
}

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(_to, _from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      const mainScrollbar = document.querySelector(
        "#main-scrollbar > .el-scrollbar__wrap"
      );
      if (mainScrollbar) {
        mainScrollbar.scroll(0, 0);
      }
      return {
        left: 0,
        top: 0,
      };
    }
  },
});

router.goBackOrTo = (to: RouteLocationRaw) => {
  if (history.state.back) {
    router.back();
  } else {
    router.replace(to);
  }
};

router.goLogVersions = (
  keyHash: any,
  storeNameHash: any,
  tableNameHash: any
) => {
  if (!useSiteStore().hasAccess("site", "log")) return;
  router.push(
    useRouteSiteId({
      name: "log-versions",
      query: {
        keyHash,
        storeNameHash,
        tableNameHash: tableNameHash ?? "",
      },
    })
  );
};

router.beforeEach((to) => {
  showLoading();
  if (to.meta.anonymous) return true;
  const jwt = cookies.get("jwt_token");

  if (!jwt) {
    hideLoading();
    localStorage.setItem("returnUrl", location.pathname + location.search);
    return {
      name: "login",
      replace: true,
    };
  }

  if (to.meta.recentVisits && typeof to.meta.recentVisits == "string") {
    let icon;
    if (to.matched) {
      for (const i of to.matched) {
        if (i.meta.menu?.icon) {
          icon = i.meta.menu.icon;
          break;
        }
      }
    }
    countRecentVisits(to.meta.recentVisits, to.fullPath, icon);
  }

  // 未指定站点 清空已有的站点信息
  if (!Object.keys(to.query).find((key) => key.toLowerCase() === "siteid")) {
    useSiteStore().clear();
  }
});

router.beforeResolve(hideLoading);

router.onError((error, to) => {
  hideLoading();
  // if (
  //   error.message.includes("Failed to fetch dynamically imported module") ||
  //   error.message.includes("Importing a module script failed")
  // ) {
  //   window.location.href = router.resolve(to).href;
  // }
});

export const install: Install = (app) => {
  app.use(router);
};

declare module "vue-router" {
  interface Router {
    goBackOrTo(to: RouteLocationRaw): void;
    goLogVersions(keyHash: any, storeNameHash: any, tableNameHash?: any): void;
  }
  interface RouteMeta {
    anonymous?: boolean;
    activeMenu?: string;
    menu?: {
      display: string;
      permission?: { feature: string; action?: string };
      icon?: string;
      name?: string;
    };
  }
}
