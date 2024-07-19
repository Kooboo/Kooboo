<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";
const $t = i18n.global.t;

export const define = {
  name: "forms",
  icon: "icon-page",
  display: $t("common.form"),
  route: "forms",
  order: 70,
  permission: "form",
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
        router.push(useRouteSiteId({ name: "forms" }));
      },
    },
  ],
} as Activity;

export const open = (item: { id: string; name: string }) => {
  useDevModeStore().addTab({
    id: item.id,
    name: item.name,
    panel: FormPanel,
    icon: "icon-page",
    actions: [
      {
        name: "save",
        display: $t("common.save"),
        visible: false,
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
import Collapse from "../components/collapse.vue";
import { useFormStore } from "@/store/form";
import FileItem from "../components/file-item.vue";
import type { Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import FormPanel from "../tab-panels/form-panel.vue";
import type { PostForm } from "@/api/form/types";
import { ref } from "vue";
import EditForm from "@/views/development/forms/edit-form.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import SettingDialog from "@/views/development/forms/setting-dialog.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { router } from "@/modules/router";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import ReSubMenu from "@/views/dev-mode/components/collapse/re-sub-menu.vue";

const { t } = useI18n();
const formStore = useFormStore();
const showAddItemDialog = ref(false);
const showSettingDialog = ref(false);
const selected = ref("");
const form = ref();
const model = ref<PostForm>();
const contextMenu = ref();

const load = () => {
  formStore.loadAll();
};

const onAdd = async () => {
  model.value = await formStore.getForm();
  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  const id = await formStore.updateForm(model.value!);
  await formStore.loadAll();
  const result = formStore.external.find((f) => f.id == id);
  if (result) open(result);
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await formStore.deleteForms([id]);
};

const onSetting = (id: string) => {
  selected.value = id;
  showSettingDialog.value = true;
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
      :title="t('common.external')"
      add
      :expand-default="true"
      permission="form"
      @on-add="onAdd"
    >
      <template v-for="item in formStore.getExternalFormList">
        <FileItem
          v-if="!item.children"
          :id="item.id"
          :key="item.id"
          :title="item.name"
          :name="item.name"
          permission="form"
          :padding="12 + item.floor * 6"
          remove
          @click="open(item)"
          @remove="onRemove(item.id)"
          @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
        >
          <el-icon
            class="iconfont icon-a-setup hover:text-blue"
            @click="onSetting(item.id)"
          />
        </FileItem>
        <ReSubMenu
          v-else
          :key="item.folderName + item.floor"
          :data="item"
          permission="form"
          :show-setting="true"
          @click-handle="open($event)"
          @remove="onRemove($event)"
          @setting="onSetting($event)"
          @click-context-menu="contextMenu?.openMenu($event.event, $event.item)"
        />
      </template>
    </Collapse>

    <Collapse
      :title="t('common.embedded')"
      :expand-default="true"
      permission="form"
    >
      <FileItem
        v-for="item of formStore.embedded"
        :id="item.id"
        :key="item.id"
        :title="item.name"
        permission="form"
        remove
        @click="open(item)"
        @remove="onRemove(item.id)"
        @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
      >
        <el-icon
          class="iconfont icon-a-setup hover:text-blue"
          @click="onSetting(item.id)"
        />
      </FileItem>
    </Collapse>
    <div @click.stop>
      <el-dialog
        v-model="showAddItemDialog"
        width="600px"
        :close-on-click-modal="false"
        :title="t('common.newForm')"
        destroy-on-close
      >
        <div @keydown.enter="onSave">
          <EditForm
            v-if="model"
            ref="form"
            label-position="top"
            :model="model"
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
    <SettingDialog
      v-if="showSettingDialog"
      :id="selected"
      v-model="showSettingDialog"
    />
    <ContextMenu ref="contextMenu" :actions="actions" />
  </div>
</template>
