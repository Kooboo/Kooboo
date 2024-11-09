import { getTypes, getListByType, deletes, post, getEdit } from "@/api/code";
import type { Code, PostCode } from "@/api/code/types";
import { emptyGuid } from "@/utils/guid";
import { defineStore } from "pinia";
import { computed, ref, watch } from "vue";
import { useDevModeStore } from "./dev-mode";
import { useSiteStore } from "./site";
import { Uri } from "monaco-editor";
import type { KeyValue } from "@/global/types";
import { monaco } from "@/components/monaco-editor/userWorker";
import { removeAllModules } from "monaco-editor-ex";
import { useModuleStore } from "./module";
import { saveLogVideo } from "@/api/site-log";

export const useCodeStore = defineStore("codeStore", () => {
  const siteStore = useSiteStore();
  const devModeStore = useDevModeStore();

  const keywords = ref();
  const isRegex = ref(false);
  const ignoreCase = ref(true);
  const types = ref<Record<string, string>>({});
  const codes = ref<Code[]>([]);
  const codesInitialized = ref(false);

  const codesByType = computed(() => {
    const result: Record<string, any> = {};
    for (const key in types.value) {
      result[key] = devModeStore.getSplitNameData(
        codes.value.filter(
          (f) => f.codeType.toLowerCase() === key?.toLowerCase()
        )
      );
    }
    return result;
  });

  const loadCodes = async () => {
    if (!siteStore.site) return;
    codes.value = await getListByType("all");
    codesInitialized.value = true;
  };

  const deleteCodes = async (ids: string[]) => {
    await deletes(ids);
    for (const id of ids) {
      const code = codes.value.find((f) => f.id == id);
      if (code) {
        const model = monaco.editor.getModel(
          Uri.file(`${code.name || code.id}.ts`)
        );
        if (model) model.dispose();
      }
    }
    devModeStore.deleteTabs(ids);
    loadCodes();
  };

  const updateCode = async (model: PostCode, events?: string) => {
    if (!model.url) model.url = "";
    model.enableDiffChecker = true;
    const result = await post(model);

    if (siteStore.site.recordSiteLogVideo && events) {
      await saveLogVideo(result, events, model.isEmbedded);
    }

    return Object.assign(model, await getCode(result));
  };

  const getCode = async (
    id?: string,
    codeType?: string,
    eventType?: string
  ) => {
    id = id ?? emptyGuid;
    const result = await getEdit(codeType || "all", id);

    const rsp = {
      id: result.id,
      name: result.name || "",
      body: result.body || "",
      config: result.config,
      url: result.url,
      eventType: result.eventType || eventType,
      codeType: result.codeType || codeType,
      version: result.version,
      scriptType: result.scriptType,
      isEmbedded: result.isEmbedded,
      isDecrypted: result.isDecrypted,
    } as PostCode;

    if (rsp.codeType?.toLowerCase() === "event" && !rsp.eventType) {
      rsp.eventType = "RouteFinding";
    }

    if (rsp.codeType?.toLowerCase() === "api" && !rsp.url) {
      rsp.url = "";
    }

    return rsp;
  };

  watch(
    () => siteStore.site?.id,
    () => {
      codes.value = [];
      codesInitialized.value = false;
      useModuleStore().reset();
      removeAllModules();
    }
  );

  if (siteStore.hasAccess("code", "view")) {
    getTypes().then((r) => (types.value = r));
  }

  const scriptTypes: KeyValue[] = [
    {
      key: "Classic",
      value: "Classic",
    },
    {
      key: "Module",
      value: "Module",
    },
  ];

  return {
    types,
    codes,
    codesInitialized,
    deleteCodes,
    updateCode,
    getCode,
    loadCodes,
    keywords,
    scriptTypes,
    ignoreCase,
    isRegex,
    codesByType,
  };
});
