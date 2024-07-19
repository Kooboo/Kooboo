<script lang="ts" setup>
import { reactive, ref } from "vue";
import Container from "./components/container.vue";
import { register, isUniqueEmailName } from "@/api/user/index";
import { useRouter } from "vue-router";
import type { Rules } from "async-validator";
import {
  emailRule,
  confirmPasswordRule,
  requiredRule,
  passwordRule,
  isUniqueNameRule,
  usernameRules,
  passwordLengthRule,
} from "@/utils/validate";
import { useFirstInputFocus } from "@/hooks/use-first-input-focus";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";
import { vscodeLogin } from "@/utils/common";

const { t } = useI18n();
const showCodeDialog = ref(false);
useFirstInputFocus();
const model = reactive({
  username: "",
  password: "",
  confirmPassword: "",
  email: "",
  tel: "",
  code: "",
});
const rules = {
  username: usernameRules,
  email: [
    emailRule,
    requiredRule(t("common.inputEmailTips")),
    isUniqueNameRule(isUniqueEmailName, t("common.theEmailAlreadyExists")),
  ],
  password: [
    requiredRule(t("common.inputPasswordTips")),
    passwordLengthRule,
    passwordRule,
  ],
  confirmPassword: [
    requiredRule(t("common.inputConfirmPasswordTips")),
    confirmPasswordRule(model),
  ],
} as unknown as Rules;
const form = ref();
const router = useRouter();

const passwordValidator = () => {
  if (model.confirmPassword.length > 0) {
    form.value.validateField("confirmPassword");
  }
};
const onRegister = async () => {
  await form.value.validate();
  await register(model);
  if (!model.code) {
    showCodeDialog.value = true;
    return;
  }
  if (getQueryString("vscode-require-auth")) {
    vscodeLogin();
  } else {
    router.replace({
      name: "home",
    });
  }
};
</script>

<template>
  <div class="h-full overflow-hidden">
    <el-scrollbar>
      <Container
        :title="t('common.register')"
        back
        @keypress.enter="onRegister"
      >
        <el-form ref="form" label-position="top" :model="model" :rules="rules">
          <el-form-item :label="t('common.username')" prop="username">
            <el-input v-model="model.username" data-cy="username" />
          </el-form-item>
          <el-form-item :label="t('common.email')" prop="email">
            <el-input v-model="model.email" data-cy="email" />
          </el-form-item>
          <el-form-item ref="psw" :label="t('common.password')" prop="password">
            <el-input
              v-model="model.password"
              type="password"
              data-cy="password"
              @blur="passwordValidator"
            />
          </el-form-item>
          <el-form-item
            :label="t('common.confirmPassword')"
            prop="confirmPassword"
          >
            <el-input
              v-model="model.confirmPassword"
              type="password"
              data-cy="confirm-password"
            />
          </el-form-item>
        </el-form>
        <el-button
          class="w-full"
          round
          type="primary"
          size="large"
          data-cy="register"
          @click.prevent="onRegister"
          >{{ t("common.register") }}</el-button
        >
        <p class="text-s pt-8 dark:text-fff/50">
          {{ t("common.registerTips") }}
          <a
            class="text-s pt-8 dark:text-fff/50"
            href="https://www.kooboo.com/terms"
          >
            {{ t("common.termsandcondition") }}</a
          >
        </p>
      </Container>
      <el-dialog
        v-model="showCodeDialog"
        width="400px"
        :close-on-click-modal="false"
        :title="t('common.validation')"
        @closed="model.code = ''"
      >
        <div class="space-y-16">
          <el-alert
            :title="t('common.registerVerifyCodeSentTip')"
            type="info"
            :closable="false"
          />
          <el-input
            v-model="model.code"
            :placeholder="t('common.verifyCode')"
          />
        </div>
        <template #footer>
          <el-button
            type="primary"
            class="w-full"
            :disabled="!model.code"
            @click="onRegister"
            >{{ t("common.confirm") }}</el-button
          >
        </template>
      </el-dialog>
    </el-scrollbar>
  </div>
</template>
