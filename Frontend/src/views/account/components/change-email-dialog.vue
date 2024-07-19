<template>
  <el-dialog
    :close-on-click-modal="false"
    :model-value="show"
    :title="props.email ? t('common.changeEmail') : t('common.bindEmail')"
    width="400px"
    custom-class="!overflow-visible"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      :model="model"
      :rules="rules"
      label-position="top"
      @submit.prevent
      @keypress.enter="change"
      @validate="handleValidate"
    >
      <el-form-item :label="t('common.email')" prop="email">
        <el-input v-model="model.email" data-cy="email" />
      </el-form-item>
      <el-form-item :label="t('common.securityCode')" prop="code">
        <div class="flex items-center space-x-8">
          <el-input
            v-model="model.code"
            class="phone_login__code_input"
            maxlength="4"
            data-cy="security-code"
          />
          <VerifyButton
            :disabled="emailInvalid"
            data-cy="send-security-code"
            @click="sendCode"
          />
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="props.email ? t('common.change') : t('common.bind')"
        @confirm="change"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref, inject, computed, reactive } from "vue";
import { useI18n } from "vue-i18n";
import {
  codeRule,
  emailRule,
  requiredRule,
  isUniqueEmailRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import VerifyButton from "@/views/account/components/verify-button.vue";
import type { Load } from "../profile.vue";
import { updateEmail } from "@/api/user/index";
import { EmailCode } from "@/api/password/index";
const props = defineProps<{
  modelValue: boolean;
  email: string;
}>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();
const show = ref(true);
const form = ref();
const model = reactive<{ email: string; code: string }>({
  email: "",
  code: "",
});

const rules = computed(() => {
  return {
    email: [
      emailRule,
      requiredRule(t("common.inputEmailTips")),
      isUniqueEmailRule(props.email),
    ],
    code: [requiredRule(t("common.inputSecurityCodeTips")), codeRule],
  } as Rules;
});

const emailInvalid = ref(true);
const handleValidate = (field: string | string[], valid: boolean) => {
  if (field === "email") {
    emailInvalid.value = !valid;
  }
};

const sendCode = async () => {
  await EmailCode(model.email);
};

const change = async () => {
  try {
    form.value.validate();
    await updateEmail(model.email, +model.code);
    reloadUser();
    show.value = false;
  } catch {
    // Do Nothing
  }
};
const reloadUser = inject("reloadUser") as Load;
</script>
