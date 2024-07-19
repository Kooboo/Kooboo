<script lang="ts" setup>
import type { ModuleFileInfo, ResourceType } from "@/api/module/types";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useMailModuleEditorStore } from "@/store/mail-module-editor";
import { getQueryString, openInNewTab } from "@/utils/url";
import { rangeRule, requiredRule, simpleNameRule } from "@/utils/validate";
import { ElMessage } from "element-plus";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import Collapse from "./collapse.vue";
import FileItem from "./file-item.vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ImgPanel from "@/views/dev-mode/tab-panels/img-panel.vue";
import BinaryPanel from "@/views/dev-mode/tab-panels/binary-panel.vue";
import ModulePanel from "./module-panel.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import { copyText } from "@/hooks/use-copy-text";

const { t } = useI18n();
const mailModuleEditorStore = useMailModuleEditorStore();
const panelFocus = ref(false);
const id = getQueryString("id");
const showAddItemDialog = ref(false);
const form = ref();
const contextMenu = ref();

const rules = {
  name: [
    requiredRule(t("common.nameRequiredTips")),
    rangeRule(1, 50),
    simpleNameRule(),
  ],
};
const model = ref({
  name: "",
  extension: "",
});
const type = ref<ResourceType>();

const onAdd = async (value: ResourceType) => {
  type.value = value;
  model.value.extension = value.defaultExtension;
  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  let name = model.value.name;
  await mailModuleEditorStore.createModuleFile(name, type.value!);

  const file = mailModuleEditorStore.files.find(
    (f) =>
      f.name == model.value!.name + f.extension &&
      f.objectType == type.value?.name
  );

  if (file) openText(file);
  showAddItemDialog.value = false;
};

const onRemove = async (name: string, type: string, id: string) => {
  await showDeleteConfirm();
  await mailModuleEditorStore.deleteModuleFile(name, type);
  mailModuleEditorStore.deleteTab(id);
};

const open = (file: ModuleFileInfo) => {
  if (file.isBinary) {
    openBinary(file);
  } else {
    openText(file);
  }
};

const openBinary = (file: ModuleFileInfo) => {
  mailModuleEditorStore.addTab({
    id: file.id,
    name: file.name,
    panel: file.objectType === "img" ? ImgPanel : BinaryPanel,
    icon: "icon-classification",
    params: {
      url: file.previewUrl,
      type: file.objectType,
      moduleId: id,
    },
  });
};

const openText = (file: ModuleFileInfo) => {
  const type = mailModuleEditorStore.types.find(
    (f) => f.name == file.objectType
  );

  const tab = {
    id: file.id,
    name: file.name,
    panel: ModulePanel,
    icon: "icon-classification",
    params: {
      type,
      file,
      moduleId: id,
    },
    actions: [
      {
        name: "save",
        visible: false,
        icon: "icon-preservation",
        display: t("common.save"),
        invoke: async () => {
          const tab = await useMailModuleEditorStore().activeTab?.panelInstance
            ?.promise;
          if (!tab) return;
          tab.save();
        },
      },
      {
        name: "refresh",
        display: t("common.refresh"),
        visible: true,
        icon: "icon-Refresh",
        invoke: async () => {
          const tab = await useMailModuleEditorStore().activeTab?.panelInstance
            ?.promise;
          if (!tab) return;
          tab.load();
        },
      },
    ],
  };

  if (file.objectType !== "root") {
    tab.actions.push({
      name: "open",
      display: t("common.preview"),
      visible: true,
      icon: "icon-share",
      invoke: async () => {
        openInNewTab(file.previewUrl);
      },
    });
  }

  mailModuleEditorStore.addTab(tab);
};

function getFile(event: any, value: ResourceType) {
  var file = event.target.files[0];

  if (!file) {
    return;
  }

  var reader = new FileReader();
  reader.readAsDataURL(file);
  reader.onload = function (e: any) {
    var b64 = e.target.result;
    mailModuleEditorStore.updateMailModuleModule(
      file.name,
      value.name,
      b64.split("base64,")[1]
    );
  };
}

function clearModel() {
  model.value = {
    name: "",
    extension: "",
  };
}

const actions = computed(() => {
  var list = [
    {
      name: t("common.copyTitle"),
      invoke: async (item: any) => {
        copyText(item.name);
      },
    },
  ];

  if (
    contextMenu.value?.payload?.previewUrl &&
    contextMenu.value?.payload?.objectType != "root"
  ) {
    list.push({
      name: t("common.preview"),
      invoke: async (item: any) => {
        openInNewTab(item.previewUrl);
      },
    });
  }

  return list;
});
</script>

<template>
  <div
    class="h-full bg-fff dark:(bg-[#252526] border-none) flex border-r border-solid border-line dark:border-[#f00]"
  >
    <div class="w-200px h-full relative">
      <div
        class="ellipsis h-36px text-black flex items-center pr-12 space-x-12 dark:text-fff/60"
      >
        <div
          class="w-36px h-full flex justify-center items-center hover:text-blue"
        >
          <el-icon
            class="iconfont icon-fanhui1 cursor-pointer"
            @click="$router.goBackOrTo({ name: 'mail-extensions' })"
          />
        </div>
        <div class="flex-1 truncate h-full leading-9" data-cy="tab-title">
          {{ t("common.mailModule") }}
        </div>
      </div>
      <el-scrollbar
        class="flex-1"
        :style="{
          outline: panelFocus ? '1px solid rgba(34, 150, 243,.5)' : '',
        }"
      >
        <div>
          <Collapse
            v-for="item of mailModuleEditorStore.types"
            :key="item.name"
            :title="item.displayName"
            :add="item.isText && item.name !== 'root'"
            :expand-default="true"
            @on-add="onAdd(item)"
          >
            <template #bar>
              <div
                v-if="item.isBinary"
                class="relative group flex items-center"
              >
                <ElIcon
                  class="iconfont icon-a-Cloudupload group-hover:text-blue"
                  data-cy="import"
                />
                <input
                  class="cursor-pointer absolute inset-0 !opacity-0"
                  type="file"
                  :accept="item.name == 'img' ? 'image/*' : ''"
                  data-cy="upload"
                  @change="getFile($event, item)"
                />
              </div>
            </template>
            <FileItem
              v-for="i of mailModuleEditorStore.files.filter(
                (f) => f.objectType === item.name
              )"
              :id="i.id"
              :key="i.name"
              :title="i.name"
              :remove="i.objectType !== 'root'"
              @remove="onRemove(i.name, i.objectType, i.id)"
              @click="open(i)"
              @contextmenu.prevent.stop="contextMenu?.openMenu($event, i)"
            />
          </Collapse>
        </div>
      </el-scrollbar>
    </div>
    <el-dialog
      v-model="showAddItemDialog"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.newModuleType', { type: type?.displayName })"
      destroy-on-close
      @close="clearModel"
    >
      <Alert
        v-if="
          type?.name === 'read' ||
          type?.name === 'compose' ||
          type?.name === 'backend'
        "
        class="-mx-32 -mt-24 mb-12"
        :title="t('common.moduleViewEntrance')"
        :content="t('common.moduleViewEntranceTips')"
      />
      <div @keydown.enter="onSave">
        <el-form
          v-if="model"
          ref="form"
          label-position="top"
          :model="model"
          :rules="rules"
          @submit.prevent
        >
          <el-form-item :label="t('common.name')" prop="name">
            <div class="w-full flex space-x-4">
              <el-input
                v-model="model.name"
                class="flex-1"
                data-cy="module-file-name"
              />
            </div>
          </el-form-item>
        </el-form>
      </div>
      <template #footer>
        <DialogFooterBar
          @confirm="onSave"
          @cancel="showAddItemDialog = false"
        />
      </template>
    </el-dialog>
    <ContextMenu ref="contextMenu" :actions="actions" />
  </div>
</template>

<style scoped>
.side-icon {
  @apply h-48px flex items-center justify-center cursor-pointer dark:(text-fff/60 hover:text-blue/87) hover:text-blue;
}
</style>
