<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";
const $t = i18n.global.t;

export const define = {
  name: "scripts",
  icon: "icon-js",
  display: $t("common.script"),
  order: 40,
  permission: "script",
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
        router.push(useRouteSiteId({ name: "scripts" }));
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
        const store = useScriptStore();
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
    panel: ScriptPanel,
    icon: "icon-js",
    actions: actions,
  });
};
</script>

<script lang="ts" setup>
import Collapse from "../components/collapse.vue";
import { useScriptStore } from "@/store/script";
import FileItem from "../components/file-item.vue";
import type { Action, Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import type { Script } from "@/api/script/types";
import type { Group } from "@/api/resource-group/types";
import ScriptPanel from "../tab-panels/script-panel.vue";
import ScriptGroupPanel from "../tab-panels/script-group-panel.vue";
import { computed, ref } from "vue";
import { emptyGuid } from "@/utils/guid";
import { showDeleteConfirm } from "@/components/basic/confirm";
import EditForm from "@/views/development/scripts/edit-form.vue";
import EditGroupDialog from "@/views/development/scripts/edit-group-dialog.vue";
import { useI18n } from "vue-i18n";
import { openInNewTab } from "@/utils/url";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { router } from "@/modules/router";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import { usePreviewUrl } from "@/hooks/use-preview-url";

const { t } = useI18n();
const scriptStore = useScriptStore();
const devModeStore = useDevModeStore();
const showAddItemDialog = ref(false);
const showAddGroupDialog = ref(false);
const form = ref();
const contextMenu = ref();
const model = ref<Script>();

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
    panel: ScriptGroupPanel,
    icon: "icon-js",
    actions: actions,
  });
};

const onAdd = async () => {
  model.value = {
    id: emptyGuid,
    name: "",
    body: "",
    extension: "js",
    isEmbedded: false,
    ownerObjectId: undefined as any,
  };

  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  const script = await scriptStore.updateScript(model.value!);
  open(script);
  scriptStore.loadExternal();
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await scriptStore.deleteScripts([id]);
};

const onRemoveGroup = async (id: string) => {
  await showDeleteConfirm();
  scriptStore.deleteGroups([id]);
};

const load = () => {
  scriptStore.loadAll();
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
defineExpose({ load });
</script>

<template>
  <div>
    <Collapse
      :title="t('common.external')"
      add
      :expand-default="true"
      permission="script"
      @on-add="onAdd"
    >
      <FileItem
        v-for="item of scriptStore.external"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        remove
        permission="script"
        @click="open(item)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
      />
    </Collapse>

    <Collapse
      :title="t('common.embedded')"
      :expand-default="true"
      permission="script"
    >
      <FileItem
        v-for="item of scriptStore.embedded"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        remove
        permission="script"
        @click="open(item)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
      />
    </Collapse>

    <Collapse
      :title="t('common.group')"
      add
      :expand-default="true"
      permission="script"
      @on-add="showAddGroupDialog = true"
    >
      <FileItem
        v-for="item of scriptStore.group"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        remove
        permission="script"
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
        :title="t('common.newScript')"
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
  </div>
</template>
