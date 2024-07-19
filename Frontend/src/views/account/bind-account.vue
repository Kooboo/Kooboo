<script lang="ts" setup>
import { reactive, ref } from "vue";
import Container from "./components/container.vue";
import { useRouter } from "vue-router";
import type { Rules } from "async-validator";
import { requiredRule, passwordRule } from "@/utils/validate";
import { bind as phoneBind } from "@/api/verify-code";
import { bind as oauthBind } from "@/api/oauth";
import type { BindParam } from "@/api/verify-code/types";
import { getQueryString } from "@/utils/url";
import { ElMessage } from "element-plus";
import { useFirstInputFocus } from "@/hooks/use-first-input-focus";

import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";
const { t } = useI18n();
useFirstInputFocus();
const model = reactive<BindParam>({
  userName: "",
  password: "",
  verifyid: getQueryString("id")!,
});

const rules = {
  userName: [requiredRule(t("common.inputUsernameTips"))],
  password: [requiredRule(t("common.inputPasswordTips"), false), passwordRule],
} as Rules;

const form = ref();
const router = useRouter();

const onBind = async () => {
  await form.value.validate();
  const bind = getQueryString("type") === "oauth" ? oauthBind : phoneBind;
  const result = await bind(model);

  if (result.error) {
    ElMessage.error(result.error);
    return;
  }

  if (result.access_token) {
    useAppStore().login(result.access_token);
    router.replace({
      name: "home",
    });
  }
};
</script>

<template>
  <div class="h-full overflow-hidden">
    <el-scrollbar>
      <Container :title="t('common.bindAccount')" @keypress.enter="onBind">
        <div class="flex items-center mb-16 text-m">
          <span class="text-444 mr-4">{{
            t("common.withoutKoobooAccount")
          }}</span>
          <a
            class="text-blue cursor-pointer"
            @click="
              router.push({
                name: 'bind-name',
                query: {
                  ...router.currentRoute.value.query,
                  id: model.verifyid,
                  type: getQueryString('type'),
                },
              })
            "
            >{{ t("common.bindName") }}</a
          >
        </div>
        <el-form ref="form" label-position="top" :model="model" :rules="rules">
          <el-form-item :label="t('common.username')" prop="userName">
            <el-input v-model="model.userName" data-cy="username" />
          </el-form-item>
          <el-form-item :label="t('common.password')" prop="password">
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
          data-cy="confirm"
          @click="onBind"
          >{{ t("common.ok") }}</el-button
        >
      </Container>
    </el-scrollbar>
  </div>
</template>
