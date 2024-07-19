<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";
const $t = i18n.global.t;

export const define = {
  name: "views",
  icon: "icon-view",
  display: $t("common.view"),
  route: "views",
  order: 10,
  permission: "view",
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
        router.push(useRouteSiteId({ name: "views" }));
      },
    },
  ],
} as Activity;

export const open = (item: { id: string; name: string }) => {
  useDevModeStore().addTab({
    id: item.id,
    name: item.name,
    panel: ViewPanel,
    icon: "icon-view",
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
        visible: true,
        display: $t("common.refresh"),
        icon: "icon-Refresh",
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          tab.load();
        },
      },
      {
        name: "open",
        display: $t("common.preview"),
        visible: true,
        icon: "icon-eyes",
        invoke: async () => {
          const store = useViewStore();
          let found = store.list.find((f) => f.id === item.id);
          if (!found) {
            await store.load();
            found = store.list.find((f) => f.id === item.id);
          }
          if (found) {
            const { onPreview } = usePreviewUrl();
            onPreview!(found.preview);
          }
        },
      },
    ],
  });
};
</script>

<script lang="ts" setup>
import { useViewStore } from "@/store/view";
import FileItem from "../components/file-item.vue";
import type { Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import type { PostView } from "@/api/view/types";
import ViewPanel from "../tab-panels/view-panel.vue";
import Collapse from "../components/collapse.vue";
import { ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import EditForm from "@/views/development/views/edit-form.vue";
import { useI18n } from "vue-i18n";
import { openInNewTab } from "@/utils/url";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { router } from "@/modules/router";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import ReSubMenu from "@/views/dev-mode/components/collapse/re-sub-menu.vue";
import { usePreviewUrl } from "@/hooks/use-preview-url";

const { t } = useI18n();
const viewStore = useViewStore();
const showAddItemDialog = ref(false);
const form = ref();
const model = ref<PostView>();
const contextMenu = ref();

const onAdd = async () => {
  model.value = await viewStore.getView();
  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  const view = await viewStore.updateView(model.value!);
  open(view);
  viewStore.load();
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await viewStore.deleteViews([id]);
};

const load = () => {
  viewStore.load();
};

const actions = [
  {
    name: t("common.copyTitle"),
    invoke: async (item: any) => {
      copyText(item.name);
    },
  },
  {
    name: t("common.preview"),
    invoke: async (item: any) => {
      const { onPreview } = usePreviewUrl();
      onPreview!(item.preview);
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
      permission="view"
      @on-add="onAdd"
    >
      <template v-for="item in viewStore.getViews">
        <FileItem
          v-if="!item.children"
          :id="item.id"
          :key="item.id"
          permission="view"
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
          permission="view"
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
        :title="t('common.newView')"
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
