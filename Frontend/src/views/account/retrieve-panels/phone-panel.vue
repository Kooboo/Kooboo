<script lang="ts" setup>
import { reactive, ref } from "vue";
import Container from "../components/container.vue";
import { useRouter } from "vue-router";
import type { Rules } from "async-validator";
import {
  requiredRule,
  phoneRule,
  codeRule,
  passwordRule,
  confirmPasswordRule,
  passwordLengthRule,
} from "@/utils/validate";
import VerifyButton from "../components/verify-button.vue";
import { ElMessage } from "element-plus";
import { ResetByTel, smsCode } from "@/api/password/index";
import { useFirstInputFocus } from "@/hooks/use-first-input-focus";
import Schema from "async-validator";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
useFirstInputFocus();
const model = reactive({
  phone: "",
  code: "",
  password: "",
  confirmPassword: "",
  region: "+86",
});

const rules = {
  phone: [requiredRule(t("common.inputPhoneTips")), phoneRule],
  code: [requiredRule(t("common.inputSecurityCodeTips")), codeRule],
  password: [
    requiredRule(t("common.inputPasswordTips"), false),
    passwordLengthRule,
    passwordRule,
  ],
  confirmPassword: [
    requiredRule(t("common.inputPasswordAgainTips"), false),
    confirmPasswordRule(model),
  ],
} as Rules;

const phoneValidator = new Schema({ phone: rules.phone });

const form = ref();
const router = useRouter();

const onReset = async () => {
  await form.value.validate();
  const result = await ResetByTel(
    model.region + model.phone,
    model.code,
    model.password
  );
  if (result.error) {
    ElMessage.error(result.error);
  } else {
    ElMessage.success(t("common.resetPasswordsSuccess"));
    delete router.currentRoute.value.query.type;
    router.replace({
      name: "login",
      query: {
        ...router.currentRoute.value.query,
      },
    });
  }
};

const codeDisabled = ref(true);
const validatePhone = () => {
  phoneValidator.validate({ phone: model.phone }, (error) => {
    codeDisabled.value = !!error;
  });
};
const validatePassword = async () => {
  if (model.confirmPassword.length > 0) {
    await form.value.validateField("confirmPassword");
  }
};

const sendCode = async () => {
  await smsCode(model.region + model.phone);
};
</script>

<template>
  <Container :title="t('common.retrieve')" back @keypress.enter="onReset">
    <template #back-append>
      <a
        class="text-blue cursor-pointer"
        data-cy="retrieve-by-email"
        @click="
          router.replace({
            name: 'retrieve-password',
            query: {
              ...router.currentRoute.value.query,
              type: 'email',
            },
          })
        "
        >{{ t("common.retrieveByEmail") }}</a
      >
    </template>
    <el-form ref="form" label-position="top" :model="model" :rules="rules">
      <el-form-item :label="t('common.phoneNumber')" prop="phone">
        <PhoneInput
          v-model="model.phone"
          v-model:regionCode="model.region"
          max-length="11"
          data-cy="phone-number"
          @input="validatePhone"
        />
      </el-form-item>
      <el-form-item :label="t('common.securityCode')" prop="code">
        <div class="flex items-center space-x-8">
          <el-input
            v-model="model.code"
            class="phone_retrieve__code_input"
            maxlength="4"
            data-cy="security-code"
          />
          <VerifyButton
            :disabled="codeDisabled"
            data-cy="send-security-code"
            @click="sendCode"
          />
        </div>
      </el-form-item>
      <el-form-item :label="t('common.password')" prop="password">
        <el-input
          v-model="model.password"
          type="password"
          data-cy="password"
          @blur="validatePassword"
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
    <el-button
      class="w-full text-l h-48px"
      round
      type="primary"
      size="large"
      data-cy="reset-password"
      @click="onReset"
      >{{ t("common.resetPassword") }}</el-button
    >
  </Container>
</template>
