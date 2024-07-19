import { defineStore } from "pinia";
import {
  getExternal,
  upload,
  deletes,
  getEdit as getScriptApi,
  post as postScriptApi,
  getEmbedded,
} from "@/api/script";
import {
  getGroups,
  getEdit as getGroupApi,
  updateGroup as updateGroupApi,
  deletes as deleteGroupsApi,
} from "@/api/resource-group";
import type { ScriptItem, Script } from "@/api/script/types";
import type { Group, PostGroup } from "@/api/resource-group/types";
import { computed, ref, watch } from "vue";
import { useSiteStore } from "./site";
import { useDevModeStore } from "./dev-mode";
import { saveLogVideo } from "@/api/site-log";

export const useScriptStore = defineStore("scriptStore", () => {
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();
  const external = ref<ScriptItem[]>([]);
  const group = ref<Group[]>([]);
  const embedded = ref<ScriptItem[]>([]);

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
    group.value = await getGroups("Script");
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

  const uploadScript = async (file: File) => {
    const formData = new FormData();
    formData.append("file", file);
    await upload(formData);
    loadExternal();
  };

  const getScript = async (id: string) => {
    const result = await getScriptApi(id);
    if (!result.body) result.body = "";
    return result;
  };

  const updateScript = async (model: Script, events?: string) => {
    model.enableDiffChecker = true;
    const result = await postScriptApi(model);

    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events, model.isEmbedded);
    }

    return Object.assign(model, await getScript(result));
  };

  const deleteScripts = async (ids: string[]) => {
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
    embedded,
    group,
    all,
    getGroup,
    updateGroup,
    deleteGroups,
    uploadScript,
    deleteScripts,
    getScript,
    updateScript,
    loadAll,
    loadEmbedded,
    loadExternal,
    loadGroups,
  };
});
