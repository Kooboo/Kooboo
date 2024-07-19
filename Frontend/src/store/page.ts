import type { Page, PostPage, PostRichPage } from "@/api/pages/types";
import { computed, ref, watch } from "vue";
import { deletes, getAll, getEdit, post, postRichText } from "@/api/pages";

import { defineStore } from "pinia";
import { emptyGuid } from "@/utils/guid";
import { useDevModeStore } from "./dev-mode";
import { useSiteStore } from "./site";
import { saveLogVideo } from "@/api/site-log";

export const usePageStore = defineStore("pageStore", () => {
  const list = ref<Page[]>([]);
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();

  const getNormalPage = computed(() => {
    return devModeStore.getSplitNameData(
      list.value.filter(
        (f) =>
          ["Normal", "Designer"].includes(f.type) && f.layoutId === emptyGuid
      )
    ) as any[];
  });
  const getLayoutPage = computed(() => {
    return devModeStore.getSplitNameData(
      list.value.filter((f) => f.layoutId !== emptyGuid)
    ) as any[];
  });
  const getRichPage = computed(() => {
    return devModeStore.getSplitNameData(
      list.value.filter((f) => f.type === "RichText")
    ) as any[];
  });

  const load = async () => {
    const allPages = await getAll();
    list.value = allPages;
    return allPages;
  };

  const deletePages = async (ids: string[]) => {
    await deletes(ids);
    devModeStore.deleteTabs(ids);
    load();
  };

  const updateRichPage = async (page: PostRichPage, events?: string) => {
    if (!page.body) page.body = " ";
    const result = await postRichText(page);
    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events);
    }
    return result;
  };

  const getPage = async (id?: string, args?: Record<string, string>) => {
    const result = await getEdit(id || emptyGuid, args);
    if (!result.body) result.body = "";
    return result;
  };

  const updatePage = async (page: PostPage, events?: string) => {
    page.enableDiffChecker = true;
    const result = await post(page);

    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events);
    }

    return Object.assign(page, await getPage(result));
  };

  watch(
    () => siteStore.site?.id,
    () => {
      list.value = [];
    }
  );

  return {
    list,
    deletePages,
    load,
    updateRichPage,
    getPage,
    updatePage,
    getNormalPage,
    getLayoutPage,
    getRichPage,
  };
});
