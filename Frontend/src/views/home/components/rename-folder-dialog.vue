<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.renameFolder')"
    @close="handleClose"
  >
    <el-form
      ref="form"
      class="el-form--label-normal"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
    >
      <el-form-item prop="name" :label="t('common.nameYourFolderTips')">
        <el-input
          v-model="model.name"
          :placeholder="t('common.folderNameTip')"
          data-cy="folder-name"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.rename')"
        @confirm="handleRename"
        @cancel="handleClose"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { reactive, ref, watch, computed } from "vue";
import type { ElForm } from "element-plus";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type { FolderItem } from "../type";
import { renameFolder } from "@/api/site";
import {
  folderIsUniqueNameRule,
  rangeRule,
  requiredRule,
  folderNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
  folder: FolderItem;
  folders: FolderItem[];
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "rename-success", value: string): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);

const form = ref<InstanceType<typeof ElForm>>();
let model = reactive({ name: "" });

watch(
  () => visible.value,
  (val) => {
    if (val) {
      model.name = props.folder.key;
    }
  }
);

const rules = {
  name: [
    requiredRule(t("common.inputFolderNameTips")),
    folderIsUniqueNameRule(
      computed(() => props.folders.filter((i) => i.key !== props.folder.key))
    ),
    rangeRule(1, 50),
    folderNameRule,
  ],
} as Rules;

function handleRename() {
  form.value?.validate(async (valid) => {
    if (valid) {
      if (props.folder.key !== model.name) {
        await renameFolder({ name: props.folder.key, newName: model.name });
        emits("rename-success", model.name);
      }
      handleClose();
    }
  });
}
</script>
