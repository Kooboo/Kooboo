<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";
const $t = i18n.global.t;

export const define = {
  name: "styles",
  icon: "icon-css",
  display: $t("common.style"),
  order: 30,
  permission: "style",
  actions: [
    {
      name: "settings",
      display: $t("common.settings"),
      icon: "icon-a-setup",
      visible: true,
      async invoke() {
        const activity = useDevModeStore().activeActivity;
        if (!activity) return;
        const instance = await activity.panelInstance.promise;
        instance.showSettings();
      },
    },
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
        router.push(useRouteSiteId({ name: "styles" }));
      },
    },
  ],
} as Activity;

export const open = (item: {
  id: string;
  name: string;
  ownerObjectId: string;
}) => {
  const actions: Action[] = [
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
  ];

  if (item.ownerObjectId === emptyGuid) {
    actions.push({
      name: "open",
      display: $t("common.preview"),
      visible: true,
      icon: "icon-eyes",
      invoke: async () => {
        const store = useStyleStore();
        let found = store.all.find((f) => f.id === item.id);

        if (!found) {
          await Promise.all([store.loadExternal(), store.loadGroups()]);
          found = store.all.find((f) => f.id === item.id);
        }

        if (found) {
          const { onPreview } = usePreviewUrl();
          onPreview!(found.fullPath);
        }
      },
    });
  }

  useDevModeStore().addTab({
    id: item.id,
    name: item.name,
    panel: StylePanel,
    icon: "icon-css",
    actions: actions,
  });
};
</script>

<script lang="ts" setup>
import Collapse from "../components/collapse.vue";
import { useStyleStore } from "@/store/style";
import FileItem from "../components/file-item.vue";
import type { Action, Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import StylePanel from "../tab-panels/style-panel.vue";
import type { Group } from "@/api/resource-group/types";
import StyleGroupPanel from "../tab-panels/style-group-panel.vue";
import { computed, ref } from "vue";
import type { Style } from "@/api/style/types";
import { emptyGuid } from "@/utils/guid";
import EditForm from "@/views/development/styles/edit-form.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import EditGroupDialog from "@/views/development/styles/edit-group-dialog.vue";
import { useI18n } from "vue-i18n";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import { router } from "@/modules/router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import StyleSettingsDialog from "@/views/development/styles/settings-dialog.vue";

const { t } = useI18n();
const styleStore = useStyleStore();
const devModeStore = useDevModeStore();
const showAddItemDialog = ref(false);
const showAddGroupDialog = ref(false);
const showSettingsDialog = ref(false);
const form = ref();
const contextMenu = ref();
const model = ref<Style>();

const openGroup = (item: Group) => {
  const actions: Action[] = [
    {
      name: "save",
      visible: false,
      display: t("common.save"),
      icon: "icon-preservation",
      invoke: async () => {
        const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
        if (!tab) return;
        tab.save();
      },
    },
  ];

  if (item.previewUrl) {
    actions!.push({
      name: "open",
      display: t("common.preview"),
      visible: true,
      icon: "icon-eyes",
      invoke: () => {
        const { onPreview } = usePreviewUrl();
        onPreview!(item.previewUrl);
      },
    });
  }

  devModeStore.addTab({
    id: item.id,
    name: item.name,
    panel: StyleGroupPanel,
    icon: "icon-css",
    actions: actions,
  });
};

const onAdd = async () => {
  model.value = {
    id: emptyGuid,
    name: "",
    body: "",
    extension: "css",
    isEmbedded: false,
    ownerObjectId: undefined as any,
  };

  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  const style = await styleStore.updateStyle(model.value!);
  open(style);
  styleStore.loadExternal();
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await styleStore.deleteStyles([id]);
};

const onRemoveGroup = async (id: string) => {
  await showDeleteConfirm();
  styleStore.deleteGroups([id]);
};

const load = () => {
  styleStore.loadAll();
};
const actions = computed(() => {
  var list = [
    {
      name: t("common.copyTitle"),
      invoke: async (item: any) => {
        copyText(item.name);
      },
    },
  ];

  var previewUrl =
    contextMenu.value?.payload?.fullUrl ||
    contextMenu.value?.payload?.previewUrl;

  if (previewUrl && !contextMenu.value?.payload?.isEmbedded) {
    list.push({
      name: t("common.preview"),
      invoke: async (item: any) => {
        const { onPreview } = usePreviewUrl();
        onPreview!(previewUrl);
      },
    });
  }

  return list;
});

function showSettings() {
  showSettingsDialog.value = true;
}

defineExpose({ load, showSettings });
</script>

<template>
  <div>
    <Collapse
      :title="t('common.external')"
      add
      :expand-default="true"
      permission="style"
      @on-add="onAdd"
    >
      <FileItem
        v-for="item of styleStore.external"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        remove
        permission="style"
        @click="open(item)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
      />
    </Collapse>

    <Collapse
      :title="t('common.embedded')"
      :expand-default="true"
      permission="style"
    >
      <FileItem
        v-for="item of styleStore.embedded"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        remove
        permission="style"
        @click="open(item)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
      />
    </Collapse>

    <Collapse
      :title="t('common.group')"
      add
      :expand-default="true"
      permission="style"
      @on-add="showAddGroupDialog = true"
    >
      <FileItem
        v-for="item of styleStore.group"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        remove
        permission="style"
        @click="openGroup(item)"
        @remove="onRemoveGroup(item.id)"
        @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
      />
    </Collapse>
    <div @click.stop>
      <el-dialog
        v-model="showAddItemDialog"
        width="600px"
        :close-on-click-modal="false"
        :title="t('common.newStyle')"
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
    <EditGroupDialog v-if="showAddGroupDialog" v-model="showAddGroupDialog" />
    <ContextMenu ref="contextMenu" :actions="actions" />
    <StyleSettingsDialog
      v-if="showSettingsDialog"
      v-model="showSettingsDialog"
    />
  </div>
</template>
