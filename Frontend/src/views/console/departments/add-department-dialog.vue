<template>
  <el-dialog
    v-model="show"
    width="400px"
    :title="t('common.addDepartment')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      :modal="model"
      :rules="newUserRules"
      label-position="top"
      :model="model"
      @submit.prevent
    >
      <el-form-item prop="name" :label="t('common.name')">
        <el-input v-model="model.name" @keydown.enter="handleAdd" />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('common.add')"
        :disabled="!model.name"
        @confirm="handleAdd"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { addDepartment } from "@/api/organization/department";
import { requiredRule } from "@/utils/validate";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

defineProps<{ modelValue: boolean }>();
const form = ref();
const model = ref({
  name: "",
});
const show = ref(true);
const newUserRules = ref({
  name: [requiredRule(t("common.fieldRequiredTips"))],
});

const handleAdd = async () => {
  await form.value.validate();
  await addDepartment(model.value);
  show.value = false;
  emit("reload");
};
</script>
