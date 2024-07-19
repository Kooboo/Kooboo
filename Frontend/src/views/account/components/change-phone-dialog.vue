<template>
  <el-dialog
    :model-value="show"
    :title="props.phone.tel ? t('common.changePhone') : t('common.bindPhone')"
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
      <el-form-item prop="phone" :label="t('common.phone')">
        <PhoneInput
          v-model="model.phone"
          v-model:regionCode="model.regionCode"
          data-cy="phone-number"
        />
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
            :disabled="phoneValid"
            data-cy="send-security-code"
            @click="sendCode"
          />
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :confirm-label="props.phone.tel ? t('common.change') : t('common.bind')"
        @confirm="change"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref, inject, computed } from "vue";
import { useI18n } from "vue-i18n";
import {
  codeRule,
  isUniquePhoneRule,
  phoneRule,
  requiredRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import VerifyButton from "@/views/account/components/verify-button.vue";
import { sendTelCode, updateTel } from "@/api/user";
import type { Load } from "../profile.vue";

const props = defineProps<{
  modelValue: boolean;
  phone: {
    tel: string;
  };
}>();
const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const { t } = useI18n();
const show = ref(true);
const form = ref();
const model = ref<{ phone: string; code: string; regionCode: string }>({
  phone: "",
  code: "",
  regionCode: "",
});

const rules = computed(() => {
  return {
    phone: [
      phoneRule,
      requiredRule(t("common.inputPhoneTips")),
      isUniquePhoneRule(model.value.regionCode, props.phone.tel),
    ],
    code: [requiredRule(t("common.inputSecurityCodeTips")), codeRule],
  } as Rules;
});
const phoneValid = ref(true);
const handleValidate = (field: string | string[], valid: boolean) => {
  if (field === "phone") phoneValid.value = !valid;
};

const sendCode = async () => {
  await sendTelCode(model.value.regionCode + model.value.phone);
};
const load = () => {
  if (!props.phone) return;
  model.value.regionCode = "+86";
};

const change = async () => {
  await form.value.validate();
  await updateTel(
    model.value.regionCode + model.value.phone,
    Number(model.value.code)
  );
  reloadUser();
  show.value = false;
};
load();
const reloadUser = inject("reloadUser") as Load;
</script>
