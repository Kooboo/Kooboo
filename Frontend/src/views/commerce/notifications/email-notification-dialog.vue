<script setup lang="ts">
import { nextTick, ref, watch } from "vue";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import {
  emailPreview,
  type EmailEvent,
  type EmailNotification,
} from "@/api/commerce/settings";
import { requiredRule } from "@/utils/validate";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import SelectInput from "@/views/mail/components/select-input/index.vue";

interface PropsType {
  modelValue: boolean;
  events: EmailEvent[];
  model?: EmailNotification;
}
interface EmitsType {
  (e: "update:model-value", value: boolean): void;
  (e: "success", value: EmailNotification): void;
}

const props = defineProps<PropsType>();
const emits = defineEmits<EmitsType>();
const form = ref();
const { t } = useI18n();
const show = ref(true);
const partner = ref(!!props.model?.sendToAddresses?.length);
const type = ref("preview");
const preview = ref<EmailNotification>();

const types = [
  {
    key: "preview",
    value: t("common.preview"),
  },
  {
    key: "code",
    value: t("common.code"),
  },
];

const copedModel = ref<EmailNotification>(
  JSON.parse(
    JSON.stringify(
      props.model ?? {
        event: "",
        subjectTemplate: "",
        bodyTemplate: "",
        sendToCustomer: true,
        sendToAddresses: [],
      }
    )
  )
);

const rules = {
  event: [requiredRule(t("common.eventRequiredTips"))],
};

async function handleSave() {
  await form.value.validate();
  emits("success", copedModel.value);
  show.value = false;
}

watch(
  [() => type.value, () => copedModel.value.event],
  async () => {
    if (type.value != "preview" || !copedModel.value.event) return;
    await nextTick();
    preview.value = await emailPreview(copedModel.value);
  },
  {
    immediate: true,
  }
);

watch(
  () => copedModel.value.event,
  () => {
    const event = props.events.find((e) => e.name == copedModel.value.event);
    if (event) {
      copedModel.value.subjectTemplate = event.mailSubjectTemplate;
      copedModel.value.bodyTemplate = event.mailBodyTemplate;
    }
  }
);

watch(
  () => partner.value,
  () => {
    if (!partner.value) copedModel.value.sendToAddresses = [];
  }
);
</script>
<template>
  <el-dialog
    v-model="show"
    width="800px"
    :close-on-click-modal="false"
    :title="t('common.emailNotifications')"
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
      <el-form-item :label="t('common.sendTo')">
        <div class="flex items-center space-x-8 w-full">
          <el-checkbox
            v-model="copedModel.sendToCustomer"
            class="flex-shrink-0"
            :label="t('common.customer')"
          />
          <el-checkbox
            v-model="partner"
            class="flex-shrink-0"
            :label="t('common.partner')"
          />
          <SelectInput
            v-if="partner"
            v-model="copedModel.sendToAddresses"
            class="flex-1 w-full"
            disable-query
            :input-value="copedModel.sendToAddresses"
            data-cy="bcc"
          />
        </div>
      </el-form-item>
      <template v-if="copedModel.event">
        <el-radio-group v-model="type" class="el-radio-group--rounded">
          <el-radio-button
            v-for="item of types"
            :key="item.key"
            :label="item.key"
            >{{ item.value }}</el-radio-button
          >
        </el-radio-group>
        <div
          v-if="type == 'preview'"
          class="border border-line rounded-normal p-8"
        >
          <h1>Subject: {{ preview?.subjectTemplate }}</h1>
          <el-divider class="my-12" />
          <KFrame class="h-300px" :content="preview?.bodyTemplate ?? ''" />
        </div>
        <template v-else>
          <el-form-item :label="t('common.subject')" prop="subjectTemplate">
            <el-input v-model="copedModel.subjectTemplate" />
          </el-form-item>
          <el-form-item :label="t('common.content')" prop="bodyTemplate">
            <MonacoEditor
              v-model="copedModel.bodyTemplate"
              class="border border-999"
              language="html"
              k-script
            />
          </el-form-item>
        </template>
      </template>
    </ElForm>
    <template #footer>
      <DialogFooterBar @confirm="handleSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>
