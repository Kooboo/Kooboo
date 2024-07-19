<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.newFolder')"
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
      <el-form-item prop="name" :label="t('common.nameYourFolder')">
        <el-input
          v-model="model.name"
          data-cy="folder-name-input"
          @keydown.enter="handleCreate"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.create')"
        @confirm="handleCreate"
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
import type { MediaFolderItem } from "@/api/content/media";
import { createMediaFolder } from "@/api/content/media";
import { createFileFolder } from "@/api/content/file";
import {
  folderIsUniqueNameRule,
  rangeRule,
  requiredRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
  path: string;
  folders: MediaFolderItem[];
  folderType?: "File";
  provider: string;
}
interface EmitsType {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
  (e: "create-success"): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const { t } = useI18n();
const { visible, handleClose } = useOperationDialog(props, emits);

const form = ref<InstanceType<typeof ElForm>>();
let model = reactive({ name: "" });
const folders = computed(() =>
  props.folders.map(({ name }) => ({ key: name }))
);

const rules = {
  name: [
    requiredRule(t("common.inputFolderNameTips")),
    rangeRule(1, 50),
    simpleNameRule(),
    folderIsUniqueNameRule(folders),
  ],
} as Rules;
watch(
  () => visible.value,
  (val) => val && form.value?.resetFields()
);
function handleCreate() {
  form.value?.validate(async (valid) => {
    if (valid) {
      const folderValue = {
        name: model.name,
        path: props.path,
        provider: props.provider ?? "default",
      };
      if (props.folderType === "File") {
        await createFileFolder(folderValue);
      } else {
        await createMediaFolder(folderValue);
      }
      handleClose();
      emits("create-success");
    }
  });
}
</script>
