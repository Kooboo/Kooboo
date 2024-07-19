<script lang="ts">
import { i18n } from "@/modules/i18n";
import { copyText } from "@/hooks/use-copy-text";
import { usePreviewUrl } from "@/hooks/use-preview-url";
const $t = i18n.global.t;

export const define = {
  name: "code",
  icon: "icon-code",
  display: $t("common.code"),
  route: "code",
  order: 25,
  permission: "code",
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
        router.push(useRouteSiteId({ name: "code" }));
      },
    },
  ],
} as Activity;

export const open = (item: { id: string; name: string; codeType: string }) => {
  const actions: Action[] = [
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
  ];

  if (item.codeType === "Api") {
    actions.push({
      name: "open",
      display: $t("common.preview"),
      visible: true,
      icon: "icon-eyes",
      invoke: async () => {
        const store = useCodeStore();
        let found = store.codes.find((f) => f.id === item.id);

        if (!found) {
          await store.loadCodes();
          found = store.codes.find((f) => f.id === item.id);
        }

        if (found) {
          const { onPreview } = usePreviewUrl();
          onPreview!(found.previewUrl);
        }
      },
    });
  }

  useDevModeStore().addTab({
    id: item.id,
    name: item.name,
    panel: CodePanel,
    icon: "icon-code",
    actions: actions,
  });
};
</script>

<script lang="ts" setup>
import Collapse from "../components/collapse.vue";
import { useCodeStore } from "@/store/code";
import FileItem from "../components/file-item.vue";
import type { Action, Activity } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import type { PostCode } from "@/api/code/types";
import CodePanel from "../tab-panels/code-panel.vue";
import { computed, ref, inject } from "vue";
import type { KeyValue } from "@/global/types";
import EditForm from "@/views/development/code/edit-form.vue";
import { emptyGuid } from "@/utils/guid";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { router } from "@/modules/router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useRoute } from "vue-router";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import ReSubMenu from "@/views/dev-mode/components/collapse/re-sub-menu.vue";
import type { UpdateTabModel } from "../index.vue";

const { t } = useI18n();
const route = useRoute();
const codeStore = useCodeStore();
const showAddItemDialog = ref(false);
const selectType = ref<KeyValue>();
const model = ref<PostCode>();
const form = ref();
const contextMenu = ref();

const load = async () => {
  await codeStore.loadCodes();
};

const onAdd = async (item: KeyValue) => {
  selectType.value = item;
  model.value = await codeStore.getCode(undefined, item.key);
  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  const code = await codeStore.updateCode(model.value!);
  open(code);
  codeStore.loadCodes();
  //更新tab 的 url和scriptType
  updateModel(code);
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await codeStore.deleteCodes([id]);
};

const onEdit = async (id: string, item: KeyValue) => {
  selectType.value = item;
  model.value = await codeStore.getCode(id, item.key);
  showAddItemDialog.value = true;
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

  if (contextMenu.value?.payload?.codeType == "Api") {
    list.push({
      name: t("common.preview"),
      invoke: async (item: any) => {
        const { onPreview } = usePreviewUrl();
        onPreview!(item.previewUrl);
      },
    });
  }

  return list;
});
defineExpose({ load });
const updateModel = inject("updateTabModel") as UpdateTabModel;
</script>

<template>
  <div>
    <Collapse
      v-for="(value, key) of codeStore.types"
      :key="key"
      permission="code"
      :title="value"
      add
      :expand-default="true"
      @on-add="onAdd({ key, value })"
    >
      <template v-if="key === 'pageScript'">
        <FileItem
          v-for="item in codeStore.codes.filter(
            (f) => f.codeType === 'PageScript'
          )"
          :id="item.id"
          :key="item.id"
          :title="item.name"
          :name="item.name"
          remove
          permission="code"
          @click="open(item)"
          @remove="onRemove(item.id)"
          @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
        >
          <el-icon
            v-if="!item.isEmbedded"
            v-hasPermission="{
              feature: route.query.activity,
              action: 'edit',
              effect: 'hiddenIcon',
            }"
            class="iconfont icon-a-writein hover:text-blue"
            @click="onEdit(item.id, { key, value })"
          />
        </FileItem>
      </template>

      <template v-for="item in codeStore.codesByType[key]" v-else>
        <FileItem
          v-if="!item.children"
          :id="item.id"
          :key="item.id"
          :title="item.name"
          :name="item.name"
          remove
          permission="code"
          :padding="12 + item.floor * 6"
          @click="open(item)"
          @remove="onRemove(item.id)"
          @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
        >
          <el-icon
            v-if="item.codeType != 'CodeBlock'"
            v-hasPermission="{
              feature: route.query.activity,
              action: 'edit',
              effect: 'hiddenIcon',
            }"
            class="iconfont icon-a-writein hover:text-blue"
            @click="onEdit(item.id, { key, value })"
          />
        </FileItem>
        <ReSubMenu
          v-else
          :key="item.folderName + item.floor"
          :data="item"
          :show-edit="key !== 'codeBlock'"
          permission="code"
          @click-handle="open($event)"
          @edit="onEdit($event.id, { key: $event.key, value: $event.value })"
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
        :title="
          model?.id == emptyGuid
            ? t(`code.newCode`, { name: selectType?.value })
            : t(`code.editCode`, { name: selectType?.value })
        "
        destroy-on-close
      >
        <div @keydown.enter="onSave">
          <EditForm
            v-if="model"
            ref="form"
            label-position="top"
            :model="model"
            :edit-mode="model?.id !== emptyGuid"
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
