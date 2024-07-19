<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.createTable')"
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
      <el-form-item prop="name" :label="t('common.tableName')">
        <el-input
          v-model="model.name"
          data-cy="table-name"
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
import { reactive, ref, watch } from "vue";
import type { ElForm } from "element-plus";
import useOperationDialog from "@/hooks/use-operation-dialog";
import type { UPDATE_MODEL_EVENT } from "@/constants/constants";
import type { DatabaseType } from "@/api/database";
import { createTable } from "@/api/database";
import {
  rangeRule,
  requiredRule,
  letterAndDigitStartRule,
  tableIsUniqueNameRule,
} from "@/utils/validate";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
  dbType: DatabaseType;
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

const rules = {
  name: [
    requiredRule(t("common.fieldRequiredTips")),
    rangeRule(1, 50),
    tableIsUniqueNameRule(props.dbType),
    letterAndDigitStartRule(),
  ],
};
watch(
  () => visible.value,
  (val) => val && form.value?.resetFields()
);
function handleCreate() {
  form.value?.validate(async (valid) => {
    if (valid) {
      await createTable(props.dbType, { name: model.name });
      handleClose();
      emits("create-success");
    }
  });
}
</script>
