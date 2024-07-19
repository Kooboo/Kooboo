import type { Group, PostGroup } from "@/api/resource-group/types";
import type { Style, StyleItem } from "@/api/style/types";
import { computed, ref, watch } from "vue";
import {
  deletes as deleteGroupsApi,
  getEdit as getGroupApi,
  getGroups,
  updateGroup as updateGroupApi,
} from "@/api/resource-group";
import {
  deletes,
  getEmbedded,
  getExternal,
  getEdit as getStyleApi,
  post as postStyleApi,
  upload,
} from "@/api/style";

import { ElMessage } from "element-plus";
import { defineStore } from "pinia";
import { i18n } from "@/modules/i18n";
import { useDevModeStore } from "./dev-mode";
import { useSiteStore } from "./site";
import { saveLogVideo } from "@/api/site-log";

const $t = i18n.global.t;

export const useStyleStore = defineStore("styleStore", () => {
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();
  const external = ref<StyleItem[]>([]);
  const group = ref<Group[]>([]);
  const embedded = ref<StyleItem[]>([]);

  const all = computed(() => {
    const list: {
      id: string;
      name: string;
      href: string;
      fullPath: string;
    }[] = [];

    list.push(
      ...external.value.map((m) => ({
        id: m.id,
        name: m.name,
        href: m.routeName,
        fullPath: m.fullUrl,
      }))
    );

    list.push(
      ...group.value.map((m) => ({
        id: m.id,
        name: m.name,
        href: m.relativeUrl,
        fullPath: m.previewUrl,
      }))
    );

    return list;
  });

  const loadExternal = async () => {
    external.value = await getExternal();
  };

  const loadGroups = async () => {
    group.value = await getGroups("Style");
  };

  const loadEmbedded = async () => {
    embedded.value = await getEmbedded();
  };

  const loadAll = () => {
    loadExternal();
    loadGroups();
    loadEmbedded();
  };

  const getGroup = (id: string) => getGroupApi(id);

  const updateGroup = async (model: PostGroup) => {
    await updateGroupApi(model);
    loadGroups();
  };

  const deleteGroups = async (ids: string[]) => {
    await deleteGroupsApi(ids);
    devModeStore.deleteTabs(ids);
    loadGroups();
  };

  const uploadStyle = async (file: File) => {
    const name = file.name;
    const formData = new FormData();
    if (name.indexOf(".") > -1 && name.split(".").reverse()[0] === "css") {
      formData.append("file", file);
      await upload(formData);
      loadExternal();
    } else {
      ElMessage.error($t("common.fileFormatIsIncorrect"));
    }
  };

  const getStyle = async (id: string) => {
    const result = await getStyleApi(id);
    if (!result.body) result.body = "";
    return result;
  };

  const updateStyle = async (model: Style, events?: string) => {
    model.enableDiffChecker = true;
    const result = await postStyleApi(model);

    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events, model.isEmbedded);
    }

    return Object.assign(model, await getStyle(result));
  };

  const deleteStyles = async (ids: string[]) => {
    await deletes(ids);
    devModeStore.deleteTabs(ids);
    loadAll();
  };

  watch(
    () => siteStore.site?.id,
    () => {
      external.value = [];
      embedded.value = [];
      group.value = [];
    }
  );

  return {
    external,
    group,
    embedded,
    all,
    getGroup,
    updateGroup,
    deleteGroups,
    uploadStyle,
    deleteStyles,
    getStyle,
    updateStyle,
    loadAll,
    loadEmbedded,
    loadExternal,
    loadGroups,
  };
});
