import { defineStore } from "pinia";
import { computed, ref, watch } from "vue";
import { getList, deletes, getEdit, post } from "@/api/view";
import type { View, PostView } from "@/api/view/types";
import { useSiteStore } from "./site";
import { emptyGuid } from "@/utils/guid";
import { useDevModeStore } from "./dev-mode";
import { saveLogVideo } from "@/api/site-log";

export const useViewStore = defineStore("viewStore", () => {
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();
  const list = ref<View[]>([]);

  const getViews = computed(() => {
    return devModeStore.getSplitNameData(list.value) as any[];
  });

  const load = async () => {
    list.value = await getList();
  };

  const deleteViews = async (ids: string[]) => {
    await deletes(ids);
    devModeStore.deleteTabs(ids);
    load();
  };

  const getView = async (id?: string) => {
    id = id || emptyGuid;
    const result = await getEdit(id);
    if (!result.body) result.body = "";
    result.id = id;
    return result;
  };

  const updateView = async (body: PostView, events?: string) => {
    body.enableDiffChecker = true;
    const result = await post(body);

    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events);
    }

    return Object.assign(body, await getView(result));
  };

  watch(
    () => siteStore.site?.id,
    () => {
      list.value = [];
    }
  );

  return { load, list, deleteViews, getView, updateView, getViews };
});
