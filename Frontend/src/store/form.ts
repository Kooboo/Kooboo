import type { Form, PostForm } from "@/api/form/types";
import { computed, ref, watch } from "vue";
import {
  deleteValues,
  deletes,
  getEdit,
  getEmbedded,
  getExternal,
  getValues,
  post,
} from "@/api/form";

import { defineStore } from "pinia";
import { emptyGuid } from "@/utils/guid";
import { useDevModeStore } from "./dev-mode";
import { useSiteStore } from "./site";
import { saveLogVideo } from "@/api/site-log";

export const useFormStore = defineStore("formStore", () => {
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();
  const external = ref<Form[]>([]);
  const embedded = ref<Form[]>([]);

  const loadExternal = async () => {
    external.value = await getExternal();
  };

  const loadEmbedded = async () => {
    embedded.value = await getEmbedded();
  };

  const getExternalFormList = computed(() => {
    return devModeStore.getSplitNameData(external.value) as any[];
  });

  const loadAll = () => Promise.all([loadExternal(), loadEmbedded()]);

  const deleteForms = async (ids: string[]) => {
    await deletes(ids);
    devModeStore.deleteTabs(ids);
    loadAll();
  };

  const getForm = async (id?: string) => {
    id = id || emptyGuid;
    const result = await getEdit(id);
    if (!result.body) result.body = "";
    result.id = id;
    return result;
  };

  const updateForm = async (body: PostForm, events?: string) => {
    if (!body.body) body.body = " ";
    const result = await post(body);

    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events, body.isEmbedded);
    }

    return result;
  };

  const getFormValues = async (id: string, pageNr: number) => {
    id = id || emptyGuid;
    const result = await getValues(id, pageNr);
    result.id = id;
    return result;
  };

  const deleteFormValues = async (ids: string[]) => {
    await deleteValues(ids);
  };

  watch(
    () => siteStore.site?.id,
    () => {
      embedded.value = [];
      external.value = [];
    }
  );

  return {
    external,
    embedded,
    getExternalFormList,
    loadAll,
    deleteForms,
    getForm,
    updateForm,
    getFormValues,
    deleteFormValues,
  };
});
