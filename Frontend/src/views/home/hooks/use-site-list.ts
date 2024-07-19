import type { SiteItem } from "../type";
import { switchStatus, deleteSite } from "@/api/site";
import { useRouter } from "vue-router";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { i18n } from "@/modules/i18n";
import { useAppStore } from "@/store/app";

const $t = i18n.global.t;
const appStore = useAppStore();

export type ListProps = {
  data: Array<SiteItem>;
};
export type ListEmits = {
  (e: "delete", value: SiteItem): void;
  (e: "share", value: SiteItem): void;
  (e: "export", value: SiteItem): void;
  (e: "moveTo", value: SiteItem): void;
};

export function useSiteList(emits: ListEmits) {
  const router = useRouter();

  function gotoDashboard(siteId: string) {
    router.push({ name: "dashboard", query: { SiteId: siteId } });
  }

  function handleMoveToFolder(item: SiteItem) {
    emits("moveTo", item);
  }

  async function handleSiteStatus(item: SiteItem) {
    await switchStatus({ id: item.siteId });
    item.online = !item.online;
  }
  async function handleDeleteSite(item: SiteItem) {
    try {
      await showDeleteConfirm();
      await deleteSite({ id: item.siteId });
      emits("delete", item);
    } catch {
      void 0;
    }
  }
  function handleActionVisible(visible: boolean, item: SiteItem) {
    item.isHover = visible;
  }
  function handleExportSite(item: SiteItem) {
    emits("export", item);
  }
  function getMoreActions(item: SiteItem) {
    appStore.currentOrg?.isAdmin;
    const actions = [
      {
        text: $t("common.moveToFolder"),
        action: handleMoveToFolder,
      },
      {
        text: $t("common.export"),
        action: handleExportSite,
      },
      {
        text: item.online ? $t("siteList.offLine") : $t("siteList.onLine"),
        action: handleSiteStatus,
        style: item.online ? undefined : { fontWeight: "bold" },
      },
      {
        text: $t("common.delete"),
        action: handleDeleteSite,
        style: { color: "#ff7148" },
      },
    ];

    return actions;
  }
  function share(item: SiteItem) {
    emits("share", item);
  }
  function gotoDesign(item: SiteItem) {
    // TODO: go to design page
    console.log(item);
  }
  return {
    getMoreActions,
    gotoDashboard,
    handleActionVisible,
    share,
    gotoDesign,
  };
}
