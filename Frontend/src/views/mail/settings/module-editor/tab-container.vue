<script lang="ts" setup>
import { useShortcut } from "@/hooks/use-shortcuts";
import type { Tab, Action } from "@/store/dev-mode";
import { useMailModuleEditorStore } from "@/store/mail-module-editor";
import TabBar from "./tab-bar.vue";

const mailModuleEditorStore = useMailModuleEditorStore();

const onSave = async () => {
  const instance = await mailModuleEditorStore.activeTab?.panelInstance.promise;
  if (instance && instance.save) instance.save();
};

const onSetAction = (tab: Tab, name: string, params: Partial<Action>) => {
  const found = tab.actions?.find((f) => f.name === name);
  if (found) {
    for (const key in params) {
      (found as any)[key] = params[key as keyof Action];
    }
  }
};

useShortcut("save", onSave);
</script>

<template>
  <div class="h-full flex-1 relative overflow-hidden">
    <div
      v-if="!mailModuleEditorStore.tabs.length"
      class="absolute inset-0 flex items-center justify-center text-200px text-black/5 dark:(bg-[#1e1e1e] text-[#424242])"
    >
      <el-icon class="iconfont icon-code3" />
    </div>

    <TabBar v-if="mailModuleEditorStore.tabs.length" />

    <div class="h-full w-full dark:bg-[#1e1e1e]">
      <template v-for="item of mailModuleEditorStore.tabs" :key="item.id">
        <keep-alive>
          <component
            :is="item.panel"
            v-show="mailModuleEditorStore.activeTab?.id === item.id"
            :id="item.id"
            :ref="(c:any) => item.panelInstance.resolve(c)"
            :name="item.name"
            :params="item.params"
            @changed="item.changed = $event"
            @set-action="(name: string, params:Partial<Action>) => onSetAction(item, name, params)"
          />
        </keep-alive>
      </template>
    </div>
  </div>
</template>
