<script setup lang="ts">
import { ref } from "vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import type { Webhook, WebhookEvent } from "@/api/commerce/settings";
import { requiredRule, urlRule } from "@/utils/validate";

interface PropsType {
  modelValue: boolean;
  events: WebhookEvent[];
  model?: Webhook;
}
interface EmitsType {
  (e: "update:model-value", value: boolean): void;
  (e: "success", value: any[]): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const form = ref();
const { t } = useI18n();
const show = ref(true);
const copedModel = ref(
  JSON.parse(
    JSON.stringify(
      props.model ?? {
        event: "",
        url: "",
      }
    )
  )
);
const rules = {
  url: [
    urlRule(t("common.urlInvalid")),
    requiredRule(t("common.urlRequiredTips")),
  ],
  event: [requiredRule(t("common.eventRequiredTips"))],
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
    title="Webhook"
    @closed="emits('update:model-value', false)"
  >
    <ElForm
      ref="form"
      :model="copedModel"
      label-position="top"
      :rules="rules"
      class="space-y-12"
    >
      <el-form-item :label="t('common.event')" prop="event">
        <el-select
          v-model="copedModel.event"
          :placeholder="t('common.pleaseSelect')"
          class="w-full"
        >
          <el-option
            v-for="item in events"
            :key="item.name"
            :value="item.name"
            :label="item.display"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="URL" prop="url">
        <el-input v-model="copedModel.url" />
      </el-form-item>
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
