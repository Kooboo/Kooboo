import { defineStore } from "pinia";
import type { Component } from "vue";
import { ref, markRaw } from "vue";
import { Completer } from "@/utils/lang";
import type { ModuleFileInfo, ResourceType } from "@/api/module/types";
import {
  getMailModuleAllFiles,
  getMailModuleFileType,
  removeMailModuleFile,
  updateMailModuleBase64,
  updateMailModuleFile,
} from "@/api/mail/mail-module";
import { uploadMessage } from "@/components/basic/message";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const useMailModuleEditorStore = defineStore(
  "mailModuleEditorStore",
  () => {
    const types = ref<ResourceType[]>([]);
    const files = ref<ModuleFileInfo[]>([]);
    const tabs = ref<Tab[]>([]);
    const activeTab = ref<Tab>();
    let editModuleId: string;

    getMailModuleFileType().then((rsp) => (types.value = rsp));

    const addTab = (tab: Omit<Tab, "panelInstance">) => {
      (tab as Tab).panelInstance = new Completer<any>();
      const found = tabs.value.find((f) => f.id === tab.id);

      if (found) {
        activeTab.value = found;
      } else {
        tab.panel = markRaw(tab.panel);
        tabs.value.push(tab as Tab);
        activeTab.value = tab as Tab;
      }
    };

    const deleteTab = (tabId: string) => {
      const item = tabs.value.find((f) => f.id === tabId);
      if (!item) return;
      const itemIndex = tabs.value.findIndex((f) => f.id === item.id);
      if (item === activeTab.value) {
        activeTab.value = tabs.value[itemIndex === 0 ? 1 : itemIndex - 1];
      }
      tabs.value.splice(itemIndex, 1);
    };

    const deleteTabs = (ids: string[]) => {
      for (const id of ids) {
        deleteTab(id);
      }
    };

    async function loadFiles() {
      const result = await getMailModuleAllFiles(editModuleId);
      files.value = result.map((m) => ({
        ...m,
        id: `${editModuleId}_${m.objectType}_${m.fullName}`,
      }));
    }

    const initialize = async (id: string) => {
      editModuleId = id;
      files.value = [];
      tabs.value = [];
      activeTab.value = undefined;
      await loadFiles();
    };

    async function createModuleFile(name: string, type: ResourceType) {
      await updateMailModuleFile(
        type.name,
        editModuleId!,
        name,
        " ",
        $t("common.createSuccess")
      );
      await loadFiles();
    }

    const deleteModuleFile = async (name: string, type: string) => {
      await removeMailModuleFile(type, editModuleId!, name);
      loadFiles();
    };

    const updateMailModuleModule = async (
      name: string,
      type: string,
      content: string
    ) => {
      await updateMailModuleBase64(type, editModuleId!, name, content);
      loadFiles();
      uploadMessage();
    };

    return {
      tabs,
      activeTab,
      addTab,
      deleteTab,
      deleteTabs,
      types,
      files,
      initialize,
      createModuleFile,
      deleteModuleFile,
      updateMailModuleModule,
    };
  }
);

export interface Tab {
  id: string;
  name: string;
  icon?: string;
  panel: Component;
  panelInstance: Completer<any>;
  actions?: Action[];
  changed?: boolean;
  params?: Record<string, unknown>;
}

export interface Action {
  name: string;
  display: string;
  icon: string;
  visible: boolean;
  invoke: () => void;
}
