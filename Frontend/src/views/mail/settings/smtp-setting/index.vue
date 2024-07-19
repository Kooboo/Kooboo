<template>
  <div class="p-24">
    <div
      class="w-[calc(50%-8px)] bg-fff p-24 rounded-normal shadow-s-10 dark:bg-[#333]"
    >
      <el-form ref="form" label-position="top" :model="model" :rules="rules">
        <el-form-item prop="server" :label="t('common.server')">
          <el-input v-model="model.server" />
        </el-form-item>
        <el-form-item prop="port" :label="t('common.port')" class="w-100px">
          <el-input-number
            v-model="model.port"
            :controls="false"
            class="smtpPortInput"
          />
        </el-form-item>
        <el-form-item prop="userName" :label="t('common.username')">
          <el-input v-model="model.userName" />
        </el-form-item>
        <el-form-item prop="password" :label="t('common.password')">
          <el-input v-model="model.password" type="password" />
        </el-form-item>
      </el-form>
      <div class="w-full mt-12">
        <el-button round type="primary" @click="save()">
          {{ t("common.save") }}
        </el-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  commerceValueRangeRule,
  passwordRule,
  portRule,
  putIntegerNumberRule,
  requiredRule,
  urlAndIpRule,
} from "@/utils/validate";
import { getSmtpSetting, updateSmtpSetting } from "@/api/mail";
import type { Rules } from "async-validator";
import type { Smpt } from "@/api/mail/types";

const { t } = useI18n();
const form = ref();
const model = ref<Smpt>({
  server: "",
  userName: "",
  password: "",
  port: 587,
});

const rules = {
  server: [
    urlAndIpRule(t("common.urlInvalid")),
    requiredRule(t("common.serverUrlRequiredTips")),
  ],
  port: [
    portRule(),
    putIntegerNumberRule(),
    requiredRule(t("common.portRequiredTips")),
  ],
  userName: [requiredRule(t("common.inputUsernameTips"))],
  password: [
    passwordRule,
    requiredRule(t("common.inputPasswordTips")),
    commerceValueRangeRule(t("common.password"), 2, 30),
  ],
} as Rules;

const load = async () => {
  model.value = await getSmtpSetting();
};

const save = async () => {
  await form.value?.validate();
  await updateSmtpSetting(model.value);
  load();
};

load();
</script>
<!-- 隐藏数字输入框的控制按钮 -->
<style>
.smtpPortInput .el-input__inner {
  text-align: left;
}
.smtpPortInput .el-input__wrapper {
  padding-left: 11px !important;
  padding-right: 11px !important;
}
</style>
