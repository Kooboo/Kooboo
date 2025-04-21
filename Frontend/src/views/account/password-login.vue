<script lang="ts" setup>
import { reactive, ref } from "vue";
import Container from "./components/container.vue";
import { login } from "@/api/user/index";
import { useRouter, useRoute } from "vue-router";
import type { Rules } from "async-validator";
import { loginRequiredRule } from "@/utils/validate";
import OtherLoginWay from "./components/other-login-way.vue";
import { useFirstInputFocus } from "@/hooks/use-first-input-focus";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";
import { vscodeLogin } from "@/utils/common";
import requestError, { getErrorData } from "@/utils/request/error";
import TwoFAVerifyDialog from "./two-fa-verify-dialog.vue";
import { errorMessage } from "@/components/basic/message";

useFirstInputFocus();
const { t } = useI18n();
const form = ref();
const router = useRouter();
const route = useRoute();
const showTwoFAVerifyDialog = ref(false);
const twoFATip = ref("");

const model = reactive({
  username: "",
  password: "",
  remember: false,
  returnurl: "",
  code: "",
});
const load = () => {
  if (route.query.returnurl) {
    model.returnurl = route.query.returnurl as string;
  }
};
load();

const rules = {
  username: loginRequiredRule(t("common.inputUsernameOrEmailTips")),
  password: loginRequiredRule(t("common.inputPasswordTips")),
} as Rules;

function twoFaOK(value: string) {
  model.code = value;
  return onLogin();
}

const onLogin = async () => {
  await form.value.validate();
  try {
    await login(model);
    if (getQueryString("vscode-require-auth")) {
      vscodeLogin();
    } else {
      router.replace({
        name: "home",
      });
    }
    return true;
  } catch (error: any) {
    model.code = "";
    if (!error.isAxiosError) return;
    const message = getErrorData(error);
    if (message) {
      if (message.startsWith("NeedVerifyCode")) {
        const type = message.split(" ")[1];
        if (type == "email") {
          twoFATip.value = t("common.emailVerifyCodeSentTip");
        } else if (type == "tel") {
          twoFATip.value = t("common.phoneVerifyCodeSentTip");
        } else if (type == "otp") {
          twoFATip.value = t("common.optVerifyTip");
        }

        showTwoFAVerifyDialog.value = true;
        return;
      } else if (message == "VerifyCodeError") {
        errorMessage(t("common.verifyCodeInvalid"));
      } else {
        requestError.showMessage(error);
      }
    } else {
      requestError.showMessage(error);
    }
    return false;
  }
};
</script>
<template>
  <div class="h-full overflow-hidden">
    <el-scrollbar>
      <Container :title="t('common.login')" @keypress.enter="onLogin">
        <div class="flex items-center mb-24 text-m">
          <span class="text-444 dark:text-fff/50 mr-4">
            {{ t("common.withoutKoobooAccount") }}
          </span>
          <a
            class="text-blue cursor-pointer"
            data-cy="register"
            @click="
              router.push({
                name: 'register',
                query: { ...router.currentRoute.value.query },
              })
            "
          >
            {{ t("login.register") }}
          </a>
        </div>
        <el-form
          ref="form"
          label-position="top"
          :model="model"
          :rules="rules"
          hide-required-asterisk
        >
          <el-form-item :label="t('common.account')" prop="username">
            <el-input
              v-model="model.username"
              :placeholder="t('common.usernameOrEmail')"
              data-cy="username-or-email"
            />
          </el-form-item>
          <el-form-item :label="t('common.password')" prop="password">
            <template #label>
              <div class="flex items-end">
                <span>
                  {{ t("common.password") }}
                </span>
                <div class="flex-1" />
                <a
                  type="text"
                  class="text-blue cursor-pointer text-s font-normal"
                  data-cy="forgot-password"
                  @click="
                    router.push({
                      name: 'retrieve-password',
                      query: { ...router.currentRoute.value.query },
                    })
                  "
                  >{{ t("common.forgotPassword") }}</a
                >
              </div>
            </template>

            <el-input
              v-model="model.password"
              type="password"
              data-cy="password"
            />
          </el-form-item>
        </el-form>
        <el-button
          class="w-full text-l h-48px"
          round
          type="primary"
          size="large"
          data-cy="login"
          @click="onLogin"
          >{{ t("common.login") }}</el-button
        >
        <el-divider>
          <span class="text-666 text-s">{{ t("login.or") }}</span>
        </el-divider>
        <OtherLoginWay />
      </Container>
    </el-scrollbar>
  </div>
  <TwoFAVerifyDialog
    v-model="showTwoFAVerifyDialog"
    :tip="twoFATip"
    @ok="twoFaOK"
  />
</template>
<style scoped>
.el-checkbox__inner {
  opacity: 0.5;
}

.el-divider--horizontal {
  margin-top: 32px;
  margin-bottom: 30px;
}
</style>
