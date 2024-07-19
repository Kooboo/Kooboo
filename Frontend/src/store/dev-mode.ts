import { computed, markRaw, nextTick, ref, watch } from "vue";

import { Completer } from "@/utils/lang";
import type { Component } from "vue";
import { DEBUG_TAB_PREFIX } from "@/constants/constants";
import { defineStore } from "pinia";
import { router } from "@/modules/router";
import { useModuleStore } from "@/store/module";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useSiteStore } from "@/store/site";
import { createRecorder } from "@/utils/monacoRecorder";
import type { editor } from "@/components/monaco-editor/userWorker";

const sidePanels = import.meta.globEager("@/views/dev-mode/side-panels/*.vue");

export const useDevModeStore = defineStore("devModeStore", () => {
  const tabs = ref<Tab[]>([]);
  const activeTab = ref<Tab>();
  const siteStore = useSiteStore();
  const activities = ref<Activity[]>([]);
  const activeActivity = ref<Activity>();
  const activeSearch = ref("");
  const recorders: { [key: string]: ReturnType<typeof createRecorder> } = {};

  const searchChildren = (list: any[], name: string, floor: number) => {
    if (list) {
      for (let i = 0; i < list.length; i++) {
        if (
          list[i].name?.toLocaleLowerCase() === name?.toLocaleLowerCase() &&
          list[i].folderName?.toLocaleLowerCase() === name?.toLocaleLowerCase()
        )
          return list[i].children;
        continue;
      }

      list.push({
        name: name,
        folderName: name,
        children: [],
        floor: floor + 1,
      });
      return list[list.length - 1].children;
    }
  };
  const getSplitNameData = (list: any[]) => {
    return list
      .reduce((acc, curr) => {
        const dir = curr.name.split(".").filter((f: string) => f !== "");
        if (
          dir.length > 1 &&
          ["html", "css", "js", "ts"].includes(
            dir[dir.length - 1].toLowerCase()
          )
        ) {
          const extension = dir.pop();
          dir[dir.length - 1] = dir[dir.length - 1] + "." + extension;
        }
        let target: any = acc;
        for (let i = 0; i < dir.length - 1; i++) {
          target = searchChildren(target, dir[i], i);
        }

        if (target) {
          target.push({
            ...curr,
            tabName: dir[dir.length - 1],
            floor: dir.length ? dir.length : 1,
          });
        }

        return acc;
      }, [])
      ?.sort((a: any, b: any) => {
        if (a.id && !b.id) {
          return 1;
        } else if (!a.id && b.id) {
          return -1;
        } else {
          return 0;
        }
      });
  };

  const debugTab = computed(() => {
    return tabs.value.find((f) => f.id.startsWith(DEBUG_TAB_PREFIX));
  });

  const addTab = (tab: Omit<Tab, "panelInstance">) => {
    (tab as Tab).panelInstance = new Completer<any>();

    if (tab.id.startsWith(DEBUG_TAB_PREFIX)) {
      addDebugTab(tab);
    } else {
      addNormalTab(tab);
    }
  };

  const addNormalTab = (tab: Omit<Tab, "panelInstance">) => {
    const found = tabs.value.find((f) => f.id === tab.id);

    if (found) {
      activeTab.value = found;
    } else {
      tab.panel = markRaw(tab.panel);
      tabs.value.push(tab as Tab);

      activeTab.value = tab as Tab;
    }
  };

  const addDebugTab = async (tab: Omit<Tab, "panelInstance">) => {
    if (debugTab.value) {
      activeTab.value = debugTab.value;
      const instance = await activeTab.value.panelInstance.promise;
      instance.load(tab.id.replace(DEBUG_TAB_PREFIX, ""));
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
    removeTabRecord(tabId);
  };

  const deleteTabs = (ids: string[]) => {
    for (const id of ids) {
      deleteTab(id);
    }
  };

  const initialize = async () => {
    tabs.value = [];
    activities.value = [];
    const list: Activity[] = [];
    activeActivity.value = undefined;
    activeTab.value = undefined;
    await nextTick();
    for (const key in sidePanels) {
      const define = sidePanels[key].define;
      if (
        define &&
        (define.permission === "debug"
          ? siteStore.hasAccess("code", "debug")
          : siteStore.hasAccess(define.permission))
      ) {
        list.push({
          ...JSON.parse(JSON.stringify(define)),
          actions: define.actions,
          panel: markRaw(sidePanels[key].default),
          panelInstance: new Completer<any>(),
        });
      }
    }

    activities.value = list.sort((a, b) => a.order - b.order);

    activeActivity.value = activities.value[0];
  };

  const loadActivePanel = async () => {
    if (!activeActivity.value) return;
    const instance = await activeActivity.value.panelInstance.promise;
    if (instance.load) instance.load();
  };

  const selectActivity = async (name: string, isExitModule?: boolean) => {
    const item = activities.value.find((f) => f.name === name);
    if (!item) return;
    activeActivity.value = item;

    if (!activeActivity.value.init) {
      activeActivity.value.init = true;
      await loadActivePanel();
    }
    if (
      name === "modules" &&
      router.currentRoute.value.query.activity === "modules" &&
      isExitModule
    ) {
      router.push(
        useRouteSiteId({
          name: "dev-mode",
          query: {
            activity: "modules",
          },
        })
      );
      useModuleStore().exitEditModule();
    } else {
      router.replace({
        name: router.currentRoute.value.name!,
        query: {
          ...router.currentRoute.value.query,
          activity: name,
        },
      });
    }
  };

  const setActiveSearch = (id: string) => {
    activeSearch.value = id;
  };

  const addTabRecord = (
    id: string,
    monaco: editor.IStandaloneCodeEditor,
    lang: string
  ) => {
    const found = tabs.value.find((f) => f.id === id);
    if (found) {
      recorders[id] = createRecorder(id, monaco, lang);
      recorders[id].on();
    }
  };

  const saveTabRecord = (id: string) => {
    return recorders[id]?.save() || "";
  };

  const removeTabRecord = (id: string) => {
    recorders[id]?.off();
    delete recorders[id];
  };

  watch(() => siteStore.site?.id, initialize, { immediate: true });

  return {
    tabs,
    activeTab,
    addTab,
    deleteTab,
    deleteTabs,
    debugTab,
    activities,
    activeActivity,
    selectActivity,
    activeSearch,
    setActiveSearch,
    getSplitNameData,
    loadActivePanel,
    addTabRecord,
    saveTabRecord,
    removeTabRecord,
  };
});

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

export interface Activity {
  name: string;
  icon: string;
  display: string;
  panelDisplay?: string;
  panel: any;
  route?: string;
  panelInstance: Completer<any>;
  init?: boolean;
  order: number;
  actions?: Action[];
  permission: string;
}
