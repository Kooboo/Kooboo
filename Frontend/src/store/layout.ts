import { defineStore } from "pinia";
import { computed, ref, watch } from "vue";
import {
  getList,
  deletes,
  getLayout as getLayoutApi,
  post,
  isUniqueName,
} from "@/api/layout";
import type { ListItem, PostLayout } from "@/api/layout/types";
import { useSiteStore } from "./site";
import { emptyGuid } from "@/utils/guid";
import {
  rangeRule,
  requiredRule,
  isUniqueNameRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { useDevModeStore } from "./dev-mode";
import { i18n } from "@/modules/i18n";
import { saveLogVideo } from "@/api/site-log";
const $t = i18n.global.t;

export const useLayoutStore = defineStore("layoutStore", () => {
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();
  const list = ref<ListItem[]>([]);

  const getLayoutList = computed(() => {
    return devModeStore.getSplitNameData(list.value) as any[];
  });

  const load = async () => {
    list.value = await getList();
  };

  const getRules = (editMode: boolean) => {
    return {
      name: editMode
        ? []
        : [
            simpleNameRule(),
            rangeRule(1, 50),
            requiredRule($t("common.nameRequiredTips")),
            isUniqueNameRule(isUniqueName, $t("common.layoutNameExistsTips")),
          ],
    } as Rules;
  };

  const deleteLayouts = async (ids: string[]) => {
    await deletes(ids);
    devModeStore.deleteTabs(ids);
    load();
  };

  const getLayout = async (id?: string) => {
    const result = await getLayoutApi(id || emptyGuid);
    if (!result.body) result.body = "";
    return result;
  };

  const updateLayout = async (body: PostLayout, events?: string) => {
    body.enableDiffChecker = true;
    const result = await post(body);

    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events);
    }

    return Object.assign(body, await getLayout(result));
  };

  watch(
    () => siteStore.site?.id,
    () => {
      list.value = [];
    }
  );

  return {
    load,
    list,
    deleteLayouts,
    getLayout,
    updateLayout,
    getRules,
    getLayoutList,
  };
});
