<script lang="ts" setup>
import { showConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import type { Tab } from "@/store/dev-mode";
import { ref } from "vue";
import { useMailModuleEditorStore } from "@/store/mail-module-editor";

const { t } = useI18n();
const mailModuleEditorStore = useMailModuleEditorStore();
const batchClose = async (tabs: Tab[], ids: string[]) => {
  const hasSaved = tabs.some((item) => {
    return item.changed;
  });
  if (hasSaved) {
    await showConfirm(t("common.unsavedChangesLeaveTips"));
  }
  mailModuleEditorStore.deleteTabs(ids);
};

const currentTab = ref();
const visible = ref(false);
const left = ref(0);
const top = ref(0);
const trigger = ref();

const openMenu = (e: { clientY: number; clientX: number }, item: any) => {
  visible.value = true;
  top.value = e.clientY;
  left.value = e.clientX;
  currentTab.value = item;
  setTimeout(() => {
    trigger.value.click();
  }, 0);
};

const actions = [
  {
    name: t("common.close"),
    invoke: async (item: any) => {
      if (item.changed) {
        await showConfirm(t("common.unsavedChangesLeaveTips"));
      }
      mailModuleEditorStore.deleteTab(item.id);
    },
  },
  {
    name: t("common.closeToRight"),
    invoke: async (item: any) => {
      const currentIndex = mailModuleEditorStore.tabs.findIndex((i) => {
        return i.id === item.id;
      });
      const rightTabs = mailModuleEditorStore.tabs.filter(
        (i, index) => index > currentIndex
      );
      const rightIds = rightTabs.map((item) => {
        return item.id;
      });
      batchClose(rightTabs, rightIds);
    },
  },
  {
    name: t("common.closeOthers"),
    invoke: async (item: any) => {
      const ids = mailModuleEditorStore.tabs.map((item) => {
        return item.id;
      });
      const otherTabs = mailModuleEditorStore.tabs.filter(
        (i) => i.id != item.id
      );
      const otherIds = ids.filter((i) => i != item.id);
      batchClose(otherTabs, otherIds);
    },
  },
  {
    name: t("common.closeSaved"),
    invoke: async () => {
      const tabSaved = mailModuleEditorStore.tabs.filter((item) => {
        return !item.changed;
      });
      const savedId = tabSaved.map((item) => {
        return item.id;
      });
      mailModuleEditorStore.deleteTabs(savedId);
    },
  },
  {
    name: t("common.closeAll"),
    invoke: async () => {
      const ids = mailModuleEditorStore.tabs.map((item) => {
        return item.id;
      });
      batchClose(mailModuleEditorStore.tabs, ids);
    },
  },
];

defineExpose({ openMenu });
</script>

<template>
  <div
    v-if="visible"
    :style="{ top: top - 2 + 'px', left: left - 32 + 'px' }"
    class="fixed"
  >
    <el-dropdown
      trigger="click"
      @command="$event.invoke(currentTab)"
      @visible-change="visible = $event"
    >
      <div ref="trigger" class="w-64 h-8" @contextmenu.stop.prevent="" />
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item
            v-for="item of actions"
            :key="item.name"
            :command="item"
            >{{ item.name }}
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>
