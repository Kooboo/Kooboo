<template>
  <el-dialog
    v-model="dialogVisible"
    width="400px"
    :title="t('common.changePassword')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="changePasswordForm"
      :rules="changePasswordRules"
      label-position="top"
      :model="model"
      @keydown.enter="submit"
    >
      <el-form-item :label="t('common.newPassword')" prop="password">
        <el-input
          v-model="model.password"
          type="password"
          data-cy="new-password"
          @blur="passwordValidator"
        />
      </el-form-item>
      <el-form-item :label="t('common.confirmPassword')" prop="confirmPassword">
        <el-input
          v-model="model.confirmPassword"
          type="password"
          data-cy="confirm-password"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="t('profile.change')"
        @confirm="submit()"
        @cancel="dialogVisible = false"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import {
  confirmPasswordRule,
  passwordLengthRule,
  passwordRule,
  requiredRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import Schema from "async-validator";
import { changePassword } from "@/api/partner";
const changePasswordForm = ref();
const passwordValidator = async () => {
  if (model.value.confirmPassword.length > 0) {
    await changePasswordForm.value.validateField("confirmPassword");
  }
};
const props = defineProps<{
  modelValue: boolean;
  username: string;
}>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();
const { t } = useI18n();
const dialogVisible = ref(true);
const model = ref({
  confirmPassword: "",
  password: "",
});
const changePasswordRules = {
  password: [
    requiredRule(t("common.fieldRequiredTips"), false),
    passwordLengthRule,
    passwordRule,
  ],
  confirmPassword: [
    requiredRule(t("common.fieldRequiredTips"), false),
    confirmPasswordRule(model.value),
  ],
} as unknown as Rules;

const validator = new Schema(changePasswordRules);
watch(
  () => model,
  (val) => {
    if (val) {
      validator.validate(model, (error) => {
        if (error) {
          return;
        }
        changePasswordForm.value.clearValidate();
      });
    }
  },
  { deep: true }
);

const submit = async () => {
  await changePasswordForm.value.validate();
  if (model.value.password === model.value.confirmPassword) {
    const form = {
      password: model.value.password,
      username: props.username,
    };
    try {
      await changePassword({
        ...form,
      });
      dialogVisible.value = false;
    } catch (error) {
      console.log(error);
    }
  }
};
</script>
