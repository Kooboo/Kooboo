<script lang="ts" setup>
import FileItem from "../file-item.vue";
import Collapse from "../collapse.vue";
import { computed, ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { useModuleStore } from "@/store/module";
import type { ModuleFileInfo, ResourceType } from "@/api/module/types";
import { useDevModeStore } from "@/store/dev-mode";
import ModulePanel from "@/views/dev-mode/tab-panels/module-panel.vue";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import BinaryPanel from "@/views/dev-mode/tab-panels/binary-panel.vue";
import ImgPanel from "@/views/dev-mode/tab-panels/img-panel.vue";
import { rangeRule, requiredRule, simpleNameRule } from "@/utils/validate";
import { ElMessage } from "element-plus";
import { useSiteStore } from "@/store/site";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import { copyText } from "@/hooks/use-copy-text";

const { t } = useI18n();
const moduleStore = useModuleStore();
const siteStore = useSiteStore();
const devModeStore = useDevModeStore();
const showAddItemDialog = ref(false);
const form = ref();
const contextMenu = ref();
const { onPreview } = usePreviewUrl();

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
  await moduleStore.createModuleFile(name, type.value!);

  const file = moduleStore.files.find(
    (f) =>
      f.name == model.value!.name + f.extension &&
      f.objectType == type.value?.name
  );

  if (file) openText(file);
  showAddItemDialog.value = false;
};

const onRemove = async (name: string, type: string, id: string) => {
  await showDeleteConfirm();
  await moduleStore.deleteModuleFile(name, type);
  devModeStore.deleteTab(id);
};

const open = (file: ModuleFileInfo) => {
  if (file.isBinary) {
    openBinary(file);
  } else {
    openText(file);
  }
};

const openBinary = (file: ModuleFileInfo) => {
  devModeStore.addTab({
    id: file.id,
    name: file.name,
    panel: file.objectType === "img" ? ImgPanel : BinaryPanel,
    icon: "icon-classification",
    params: {
      url: file.previewUrl,
      type: file.objectType,
      moduleId: moduleStore.editingModule,
    },
  });
};

const openText = (file: ModuleFileInfo) => {
  const type = moduleStore.types.find((f) => f.name == file.objectType);

  const tab = {
    id: file.id,
    name: file.name,
    panel: ModulePanel,
    icon: "icon-classification",
    params: {
      type,
      file,
      moduleId: moduleStore.editingModule,
    },
    actions: [
      {
        name: "save",
        visible: false,
        icon: "icon-preservation",
        display: t("common.save"),
        invoke: async () => {
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
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
          const tab = await useDevModeStore().activeTab?.panelInstance?.promise;
          if (!tab) return;
          tab.load();
        },
      },
    ],
  };

  if (file.objectType !== "root" && file.objectType !== "code") {
    tab.actions.push({
      name: "open",
      display: t("common.preview"),
      visible: true,
      icon: "icon-eyes",
      invoke: async () => {
        onPreview(file.previewUrl);
      },
    });
  }

  devModeStore.addTab(tab);
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
    moduleStore.uploadModuleFile(file.name, b64.split("base64,")[1], value);
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
        onPreview(item.previewUrl);
      },
    });
  }

  return list;
});
</script>
<template>
  <div>
    <template v-for="item of moduleStore.types" :key="item.name">
      <Collapse
        :title="item.displayName"
        :add="item.isText && item.name !== 'root'"
        :expand-default="true"
        permission="module"
        @on-add="onAdd(item)"
      >
        <template #bar>
          <div class="relative group flex items-center">
            <el-icon
              v-if="item.isBinary && siteStore.hasAccess('module', 'edit')"
              class="iconfont icon-a-Cloudupload group-hover:text-blue"
              data-cy="import"
            />
            <input
              class="!cursor-pointer absolute inset-0 !opacity-0 bg-green overflow-hidden"
              type="file"
              title=" "
              :accept="item.name == 'img' ? 'image/*' : ''"
              data-cy="upload"
              @change="getFile($event, item)"
            />
          </div>
        </template>
        <FileItem
          v-for="i of moduleStore.files.filter(
            (f) => f.objectType === item.name
          )"
          :id="i.id"
          :key="i.name"
          :title="i.name"
          :remove="i.objectType !== 'root'"
          permission="module"
          @remove="onRemove(i.name, i.objectType, i.id)"
          @click="open(i)"
          @contextmenu.prevent.stop="contextMenu?.openMenu($event, i)"
        />
      </Collapse>
    </template>
    <Teleport to="body">
      <div @click.stop>
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
              type?.displayName === 'View' || type?.displayName === 'Backend'
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
    </Teleport>
  </div>
</template>
<style scoped>
input[type="file"]::-webkit-file-upload-button {
  /* chromes and blink button */
  cursor: pointer;
}
</style>
