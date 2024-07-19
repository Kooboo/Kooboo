<script lang="ts" setup>
import { reactive, ref } from "vue";
import Container from "./components/container.vue";
import { useRouter } from "vue-router";
import type { Rules } from "async-validator";
import { usernameRules } from "@/utils/validate";
import { register as phoneRegister } from "@/api/verify-code";
import { register as oauthRegister } from "@/api/oauth";
import type { BindParam } from "@/api/verify-code/types";
import { newGuid } from "@/utils/guid";
import { getQueryString } from "@/utils/url";
import { ElMessage } from "element-plus";

import { useFirstInputFocus } from "@/hooks/use-first-input-focus";
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/store/app";

const { t } = useI18n();
useFirstInputFocus();
const model = reactive<BindParam>({
  userName: "",
  password: newGuid(),
  verifyid: getQueryString("id")!,
});

const rules = {
  userName: usernameRules,
} as Rules;

const form = ref();
const router = useRouter();

const onBind = async () => {
  await form.value.validate();
  const register =
    getQueryString("type") === "oauth" ? oauthRegister : phoneRegister;
  const result = await register(model);

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
      <Container :title="t('common.pickName')" @keypress.enter="onBind">
        <div class="flex items-center mb-16 text-m">
          <span class="text-444 mr-4">{{ t("common.haveKoobooAccount") }}</span>
          <a
            class="text-blue cursor-pointer"
            data-cy="binding-account"
            @click="
              router.push({
                name: 'bind-account',
                query: {
                  ...router.currentRoute.value.query,
                  id: model.verifyid,
                  type: getQueryString('type'),
                },
              })
            "
            >{{ t("common.bindAccount") }}</a
          >
        </div>
        <el-form
          ref="form"
          label-position="top"
          :model="model"
          :rules="rules"
          @submit.prevent
        >
          <el-form-item :label="t('common.username')" prop="userName">
            <el-input v-model="model.userName" data-cy="username" />
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
