<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";
const $t = i18n.global.t;

export const define = {
  name: "layouts",
  icon: "icon-layout",
  display: $t("common.layout"),
  route: "layouts",
  order: 20,
  permission: "layout",
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
        router.push(useRouteSiteId({ name: "layouts" }));
      },
    },
  ],
} as Activity;

export const open = (item: { id: string; name: string }) => {
  useDevModeStore().addTab({
    id: item.id,
    name: item.name,
    panel: LayoutPanel,
    icon: "icon-layout",
    actions: [
      {
        name: "save",
        visible: false,
        display: $t("common.save"),
        icon: "icon-preservation",
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          tab.save();
        },
      },
      {
        name: "refresh",
        display: $t("common.refresh"),
        visible: true,
        icon: "icon-Refresh",
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          tab.load();
        },
      },
    ],
  });
};
</script>

<script lang="ts" setup>
import { useLayoutStore } from "@/store/layout";
import FileItem from "../components/file-item.vue";
import type { Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import type { PostLayout } from "@/api/layout/types";
import LayoutPanel from "../tab-panels/layout-panel.vue";
import Collapse from "../components/collapse.vue";
import { ref } from "vue";
import EditForm from "@/views/development/layouts/edit-form.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { router } from "@/modules/router";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import ReSubMenu from "@/views/dev-mode/components/collapse/re-sub-menu.vue";

const { t } = useI18n();
const layoutStore = useLayoutStore();
const showAddItemDialog = ref(false);
const form = ref();
const model = ref<PostLayout>();
const contextMenu = ref();

const onAdd = async () => {
  model.value = await layoutStore.getLayout();
  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  const layout = await layoutStore.updateLayout(model.value!);
  open(layout);
  layoutStore.load();
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await layoutStore.deleteLayouts([id]);
};

const load = () => {
  layoutStore.load();
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
      permission="layout"
      @on-add="onAdd"
    >
      <template v-for="item in layoutStore.getLayoutList">
        <FileItem
          v-if="!item.children"
          :id="item.id"
          :key="item.id"
          permission="layout"
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
          permission="layout"
          @click-handle="open($event)"
          @remove="onRemove($event)"
          @click-context-menu="contextMenu?.openMenu($event.event, $event.item)"
        />
      </template>
    </Collapse>
    <div @click.stop>
      <el-dialog
        v-model="showAddItemDialog"
        width="600px"
        :close-on-click-modal="false"
        :title="t('common.newLayout')"
        destroy-on-close
      >
        <div @keydown.enter="onSave">
          <EditForm
            v-if="model"
            ref="form"
            label-position="top"
            :model="model"
            :edit-mode="false"
          />
        </div>
        <template #footer>
          <DialogFooterBar
            @confirm="onSave"
            @cancel="showAddItemDialog = false"
          />
        </template>
      </el-dialog>
    </div>
    <ContextMenu ref="contextMenu" :actions="actions" />
  </div>
</template>
