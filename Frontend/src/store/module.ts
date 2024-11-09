import { defineStore } from "pinia";
import { ref, watch } from "vue";
import { useRoute } from "vue-router";
import {
  getList,
  createModule as createModuleApi,
  deleteModules as deleteModulesApi,
  getType,
} from "@/api/module";
import {
  getAllFiles,
  updateBase64,
  updateFile,
  removeFile,
} from "@/api/module/files";
import { useSiteStore } from "./site";
import { useDevModeStore } from "./dev-mode";
import { i18n } from "@/modules/i18n";

import type {
  ModuleFileInfo,
  ResourceType,
  ScriptModule,
} from "@/api/module/types";
import { removeAllModules } from "monaco-editor-ex";
import { monaco } from "@/components/monaco-editor/userWorker";
import { Uri } from "monaco-editor";

const $t = i18n.global.t;

export const useModuleStore = defineStore("moduleStore", () => {
  const siteStore = useSiteStore();
  const route = useRoute();
  const devModeStore = useDevModeStore();
  const list = ref<ScriptModule[]>([]);
  const types = ref<ResourceType[]>([]);
  const editingModule = ref<string>();
  const files = ref<ModuleFileInfo[]>([]);

  const modulesInitialized = ref(false);

  if (siteStore.site) {
    getType().then((r) => (types.value = r));
  }

  const load = async () => {
    if (editingModule.value) {
      const result = await getAllFiles(editingModule.value);
      files.value = result.map((m) => ({
        ...m,
        id: `${editingModule.value}_${m.objectType}_${m.fullName}`,
      }));
    } else {
      list.value = await getList();
      modulesInitialized.value = true;
    }
  };

  const createModuleFile = async (name: string, type: ResourceType) => {
    await updateFile(type.name, editingModule.value!, name, " ");
    await load();
  };

  const uploadModuleFile = async (
    name: string,
    base64: string,
    type: ResourceType
  ) => {
    await updateBase64(type.name, editingModule.value!, name, base64);
    load();
  };

  const deleteModuleFile = async (name: string, type: string) => {
    await removeFile(type, editingModule.value!, name);
    if (type == "code") {
      const moduleName = list.value.find(
        (f) => f.id == editingModule.value
      )?.name;
      const model = monaco.editor.getModel(
        Uri.parse(`file:///node_modules/@types/module:${moduleName}/${name}.ts`)
      );
      if (model) model.dispose();
    }

    load();
  };

  const deleteModules = async (ids: string[]) => {
    await deleteModulesApi(ids);
    removeAllModules();
    load();
  };

  const createModule = async (name: string) => {
    await createModuleApi(name);
    load();
  };

  const editModule = (id: string) => {
    editingModule.value = id;
    if (devModeStore.activeActivity) {
      const action = devModeStore.activeActivity.actions?.find(
        (f) => f.name === "back"
      );
      if (action) action.visible = true;
    }
    files.value = [];
    load();
  };

  const reset = () => {
    modulesInitialized.value = false;
    editingModule.value = undefined;
    if (devModeStore.activeActivity) {
      devModeStore.activeActivity.panelDisplay = $t("common.modules");
      const action = devModeStore.activeActivity.actions?.find(
        (f) => f.name === "back"
      );
      if (action) action.visible = false;
    }

    files.value = [];
  };

  const exitEditModule = () => {
    reset();
    load();
  };

  watch(() => siteStore.site?.id, reset);
  watch(() => route?.query.moduleId, reset);

  return {
    load,
    list,
    createModule,
    deleteModules,
    types,
    editingModule,
    editModule,
    files,
    createModuleFile,
    deleteModuleFile,
    uploadModuleFile,
    exitEditModule,
    reset,
    modulesInitialized,
  };
});
