<script lang="ts" setup>
import { reactive, ref, shallowRef } from "vue";
import Container from "./components/container.vue";
import { useRouter, useRoute } from "vue-router";
import type { Rules } from "async-validator";
import { requiredRule, phoneRule, codeRule } from "@/utils/validate";
import { smsCode, verifyTelCode } from "@/api/verify-code";
import VerifyButton from "./components/verify-button.vue";
import type { ElForm } from "element-plus";
import { ElMessage } from "element-plus";
import { useFirstInputFocus } from "@/hooks/use-first-input-focus";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";
import Schema from "async-validator";

const { t } = useI18n();
useFirstInputFocus();
const model = reactive({
  phone: "",
  code: "",
  regionCode: "+86",
  returnurl: "",
});

const rules = {
  phone: [requiredRule(t("common.inputPhoneTips")), phoneRule],
  code: [requiredRule(t("common.inputSecurityCodeTips")), codeRule],
} as Rules;

const phoneValidator = new Schema({ phone: rules.phone });

const form = shallowRef<InstanceType<typeof ElForm> | null>(null);
const router = useRouter();
const route = useRoute();

if (route.query.returnurl) {
  model.returnurl = route.query.returnurl as string;
}

const onLogin = async () => {
  await form.value!.validate();
  const result = await verifyTelCode(
    `${model.regionCode}${model.phone}`,
    model.code,
    model.returnurl
  );
  if (result.error) {
    ElMessage.error(result.error);
    return;
  }
  if (result.requireinfo) {
    router.replace({
      name: "bind-name",
      query: {
        ...router.currentRoute.value.query,
        id: result.verifyid,
      },
    });
  } else {
    useAppStore().login(result.access_token);
    router.replace({
      name: "home",
    });
  }
};

const phoneValid = ref(false);
const codeDisabled = ref(true);

const handleValidate = (field: string | string[], valid: boolean) => {
  if (field === "phone") phoneValid.value = valid;
};
const phoneValidate = () => {
  phoneValidator.validate({ phone: model.phone }, (error) => {
    codeDisabled.value = !!error;
  });
};

const sendCode = () => {
  smsCode(model.regionCode + model.phone);
};
</script>

<template>
  <div class="h-full overflow-hidden">
    <el-scrollbar>
      <Container :title="t('common.phoneLogin')" back @keypress.enter="onLogin">
        <el-form
          ref="form"
          label-position="top"
          :model="model"
          :rules="rules"
          hide-required-asterisk
          @validate="handleValidate"
        >
          <el-form-item :label="t('common.phoneNumber')" prop="phone">
            <PhoneInput
              v-model="model.phone"
              v-model:regionCode="model.regionCode"
              data-cy="phone-number"
              @input="phoneValidate"
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
                :disabled="codeDisabled"
                data-cy="send-security-code"
                @click="sendCode"
              />
            </div>
          </el-form-item>
        </el-form>
        <el-button
          class="w-full text-l h-48px"
          round
          type="primary"
          size="large"
          data-cy="login"
          @click="onLogin"
        >
          {{ t("common.login") }}
        </el-button>
      </Container>
    </el-scrollbar>
  </div>
</template>

<style lang="scss">
// https://stackoverflow.com/a/20941546/14835397
input::-webkit-calendar-picker-indicator {
  position: absolute;
  left: 9999px;
}
</style>
