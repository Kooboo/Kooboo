<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";
const $t = i18n.global.t;

export const define = {
  name: "menus",
  icon: "icon-menu",
  display: $t("common.menu"),
  order: 60,
  permission: "menu",
  actions: [
    {
      name: "refresh",
      display: $t("common.refresh"),
      icon: "icon-Refresh",
      visible: true,
      async invoke() {
        const activity = useDevModeStore().activeActivity;
        if (!activity) return;
        const instance = await activity.panelInstance.promise;
        instance.load();
      },
    },
    {
      name: "more",
      display: $t("common.more"),
      icon: "icon-more",
      visible: true,
      invoke() {
        router.push(useRouteSiteId({ name: "menus" }));
      },
    },
  ],
} as Activity;

export const open = (item: { id: string; name: string }) => {
  useDevModeStore().addTab({
    id: item.id,
    name: item.name,
    panel: MenuPanel,
    icon: "icon-menu",
  });
};
</script>

<script lang="ts" setup>
import { useMenuStore } from "@/store/menu";
import FileItem from "../components/file-item.vue";
import type { Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import MenuPanel from "../tab-panels/menu-panel.vue";
import Collapse from "../components/collapse.vue";
import AddDialog from "@/views/development/menus/components/add-dialog.vue";
import { ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { router } from "@/modules/router";
import ContextMenu from "@/components/basic/context-menu.vue";
import ReSubMenu from "@/views/dev-mode/components/collapse/re-sub-menu.vue";

const { t } = useI18n();
const menuStore = useMenuStore();
const showAddItemDialog = ref(false);
const contextMenu = ref();

const load = () => {
  menuStore.load();
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await menuStore.deleteMenus([id]);
};
const actions = [
  {
    name: t("common.copyTitle"),
    invoke: async (item: any) => {
      copyText(item.name);
    },
  },
];
defineExpose({ load });
</script>

<template>
  <div>
    <Collapse
      :title="t('common.list')"
      add
      :expand-default="true"
      permission="menu"
      @on-add="showAddItemDialog = true"
    >
      <template v-for="item in menuStore.getMenuList">
        <FileItem
          v-if="!item.children"
          :id="item.id"
          :key="item.id"
          permission="menu"
          :title="item.name"
          :name="item.name"
          :padding="12 + item.floor * 6"
          remove
          @click="open(item)"
          @remove="onRemove(item.id)"
          @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
        />
        <ReSubMenu
          v-else
          :key="item.folderName + item.floor"
          :data="item"
          permission="menu"
          @click-handle="open($event)"
          @remove="onRemove($event)"
          @click-context-menu="contextMenu?.openMenu($event.event, $event.item)"
        />
      </template>
    </Collapse>

    <AddDialog
      v-if="showAddItemDialog"
      v-model="showAddItemDialog"
      @created="open($event)"
    />
    <ContextMenu ref="contextMenu" :actions="actions" />
  </div>
</template>
