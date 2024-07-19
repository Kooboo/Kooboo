<script lang="ts" setup>
import FileItem from "../file-item.vue";
import Collapse from "../collapse.vue";
import { computed, ref } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { useModuleStore } from "@/store/module";
import EditForm from "@/views/modules/edit-form.vue";
import ImportDialog from "@/views/modules/import-dialog.vue";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import ContextMenu from "@/components/basic/context-menu.vue";
import { copyText } from "@/hooks/use-copy-text";

const { t } = useI18n();
const router = useRouter();

const moduleStore = useModuleStore();
const showAddItemDialog = ref(false);
const showImportDialog = ref(false);
const form = ref();
const model = ref<{ name: string }>();
const contextMenu = ref();
const showManageDialog = ref(false);
let backUrl = ref<string>();

const onAdd = async () => {
  model.value = { name: "" };
  showAddItemDialog.value = true;
};

const onSave = async () => {
  await form.value.validate();
  await moduleStore.createModule(model.value!.name);
  showAddItemDialog.value = false;
};

const onRemove = async (id: string) => {
  await showDeleteConfirm();
  await moduleStore.deleteModules([id]);
};

const onEdit = (id: string) => {
  moduleStore.editModule(id);
  router.push(
    useRouteSiteId({
      name: "dev-mode",
      query: {
        activity: "modules",
        moduleId: id,
      },
    })
  );
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

  if (contextMenu.value?.payload?.backendViewUrl) {
    list.push({
      name: t("common.backend"),
      invoke: async (item: any) => {
        backUrl.value = item.backendViewUrl;
        showManageDialog.value = true;
      },
    });
  }

  return list;
});
</script>
<template>
  <Collapse
    :title="t('common.list')"
    add
    :expand-default="true"
    permission="module"
    @on-add="onAdd"
  >
    <template #bar>
      <el-icon
        v-hasPermission="{
          feature: 'module',
          action: 'edit',
          effect: 'hiddenIcon',
        }"
        class="iconfont icon-a-Cloudupload hover:text-blue"
        @click="showImportDialog = true"
      />
    </template>
    <FileItem
      v-for="item of moduleStore.list"
      :id="item.id"
      :key="item.id"
      :title="item.name"
      remove
      permission="module"
      @remove="onRemove(item.id)"
      @click="onEdit(item.id)"
      @contextmenu.prevent.stop="contextMenu?.openMenu($event, item)"
    />
  </Collapse>
  <div @click.stop>
    <el-dialog
      v-model="showAddItemDialog"
      width="600px"
      :close-on-click-modal="false"
      :title="t('common.newModule')"
      destroy-on-close
    >
      <div @keydown.enter="onSave">
        <EditForm v-if="model" ref="form" :model="model" />
      </div>
      <template #footer>
        <DialogFooterBar
          @confirm="onSave"
          @cancel="showAddItemDialog = false"
        />
      </template>
    </el-dialog>
  </div>
  <ImportDialog v-if="showImportDialog" v-model="showImportDialog" />
  <ContextMenu ref="contextMenu" :actions="actions" />
  <el-dialog
    v-model="showManageDialog"
    width="1000px"
    :close-on-click-modal="false"
    custom-class="el-dialog--zero-padding"
    :title="t('common.manage')"
    @closed="showManageDialog = false"
  >
    <iframe :src="backUrl" class="w-full h-500px" />
  </el-dialog>
</template>
