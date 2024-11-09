<script setup lang="ts">
import { ref } from "vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import { portRule, requiredRule, urlAndIpRule } from "@/utils/validate";
import type { SmtpSetting } from "@/api/commerce/settings";

interface PropsType {
  modelValue: boolean;
  model?: SmtpSetting;
}
interface EmitsType {
  (e: "update:model-value", value: boolean): void;
  (e: "success", value: SmtpSetting): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const form = ref();
const { t } = useI18n();
const show = ref(true);

const copedModel = ref<SmtpSetting>(
  JSON.parse(
    JSON.stringify(
      props.model ?? {
        server: "",
        port: "",
        ssl: false,
        userName: "",
        password: "",
        from: "",
      }
    )
  )
);

const rules = {
  server: [
    urlAndIpRule(t("common.serverUrlInvalid")),
    requiredRule(t("common.serverUrlRequiredTips")),
  ],
  port: [portRule()],
  userName: [requiredRule(t("common.inputAccountTips"))],
  password: [requiredRule(t("common.inputPasswordTips"))],
};

async function handleSave() {
  await form.value.validate();
  emits("success", copedModel.value);
  show.value = false;
}
</script>
<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.sendingServerSettings')"
    @closed="emits('update:model-value', false)"
  >
    <ElForm
      ref="form"
      :model="copedModel"
      label-position="top"
      :rules="rules"
      class="space-y-12"
    >
      <el-form-item :label="t('common.server')" prop="server">
        <ElInput v-model="copedModel.server" placeholder="smtp.qq.com" />
      </el-form-item>
      <el-form-item :label="t('common.port')">
        <ElInputNumber v-model.number="copedModel.port" placeholder="587" />
      </el-form-item>
      <el-form-item label="SSL">
        <el-switch v-model="copedModel.ssl" />
      </el-form-item>
      <el-form-item :label="t('common.account')" prop="userName">
        <ElInput v-model="copedModel.userName" placeholder="name@example.com" />
      </el-form-item>
      <el-form-item :label="t('common.password')" prop="password">
        <ElInput v-model="copedModel.password" type="password" />
      </el-form-item>
      <el-form-item :label="t('common.fromAddress')">
        <ElInput v-model="copedModel.from" placeholder="name@example.com" />
      </el-form-item>
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
