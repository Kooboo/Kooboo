import { getKeyValueList, getSite } from "@/api/site";

import type { KeyValue } from "@/global/types";
import type { Permission } from "@/api/role/types";
import type { Ref } from "vue";
import type { ModuleMenu, Site } from "@/api/site/site";
import { defineStore } from "pinia";
import { ref } from "vue";
import { router } from "@/modules/router";

export const useSiteStore = defineStore("siteStore", () => {
  const site = ref<Site>();
  const sites = ref<KeyValue[]>([]);
  const permissions = ref<Permission[]>([]);
  const isAdmin = ref<boolean>(false);
  const firstActiveMenu = ref();
  const serviceLevel = ref();
  const moduleMenus = ref<ModuleMenu[]>([]);

  const loadSites = async () => {
    sites.value = await getKeyValueList();
  };

  const loadSite = async () => {
    try {
      const rsp = await getSite();
      site.value = rsp.site;
      permissions.value = rsp.permissions ?? [];
      isAdmin.value = rsp.isAdmin;
      serviceLevel.value = rsp.serviceLevel;
      moduleMenus.value = rsp.moduleMenus;
      site.value.baseUrl = rsp.baseUrl;
      site.value.prUrl = rsp.prUrl;
      site.value.visibleAdvancedMenus = rsp.visibleAdvancedMenus;
      site.value.tinymceToolbarSettings = rsp.site.tinymceToolbarSettings ?? "";
      site.value.enableTinymceToolbarSettings =
        rsp.site.enableTinymceToolbarSettings ?? false;
      site.value.tinymceSettings = rsp.site.tinymceSettings ?? {};

      // 兼容旧数据
      if (
        site.value.tinymceToolbarSettings &&
        !site.value.tinymceSettings.toolbar
      ) {
        site.value.tinymceSettings.toolbar = site.value.tinymceToolbarSettings;
        site.value.tinymceToolbarSettings = "";
      }

      site.value.specialPath = rsp.site.specialPath ?? [];
      if (rsp.showContinueDownload === null) {
        site.value.continueDownload = undefined;
      }
    } catch {
      router.push({ name: "home" });
    }
  };

  const clear = () => {
    site.value = undefined;
    sites.value = [];
    permissions.value = [];
    isAdmin.value = false;
  };

  const hasAccess = (feature: string, action = "view") => {
    if (isAdmin.value) return true;
    return permissions.value.some(
      (s) => s.feature === feature && s.action === action && s.access
    );
  };

  return {
    site: site as Ref<Site>,
    sites,
    permissions,
    isAdmin,
    loadSite,
    loadSites,
    hasAccess,
    clear,
    firstActiveMenu,
    serviceLevel,
    moduleMenus,
  };
});
