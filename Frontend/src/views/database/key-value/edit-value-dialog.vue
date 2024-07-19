<script setup lang="ts">
import { ref } from "vue";
import type { KeyValue } from "@/global/types";
import { isUniqueName, update, get } from "@/api/database/key-value";
import {
  isUniqueNameRule,
  letterAndDigitStartRule,
  requiredRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

interface PropsType {
  modelValue: boolean;
  current?: string;
  alert?: string;
}
interface EmitsType {
  (e: "update:modelValue", value: boolean): void;
  (e: "success", value: boolean): void;
}

const show = ref(true);
const props = defineProps<PropsType>();
const emit = defineEmits<EmitsType>();
const { t } = useI18n();
const isNew = !props.current;
const form = ref();
const success = ref(false);

const model = ref<KeyValue>({
  key: props.current!,
  value: "",
});

(async () => {
  if (!isNew) {
    model.value.value = await get(model.value.key);
  }
})();

const createModelRules = {
  value: [requiredRule(t("common.fieldRequiredTips"))],
  key: [
    requiredRule(t("common.fieldRequiredTips")),
    letterAndDigitStartRule(),
    isNew
      ? isUniqueNameRule(isUniqueName, t("common.keyHasBeenTakenTips"))
      : undefined,
  ],
} as Rules;

async function save() {
  if (!form.value) return;
  await form.value.validate();
  await update(model.value);
  success.value = true;
  show.value = false;
}

const close = () => {
  emit("success", success.value);
  emit("update:modelValue", false);
};
</script>
<template>
  <el-dialog
    v-model="show"
    width="600px"
    :close-on-click-modal="false"
    :title="isNew ? t('common.new') : t('common.edit')"
    custom-class="el-dialog--zero-padding"
    @close="close"
  >
    <Alert v-if="alert" :content="alert" />
    <div class="px-32 py-24">
      <el-form
        ref="form"
        :model="model"
        :rules="createModelRules"
        label-position="top"
        @submit.prevent
      >
        <el-form-item prop="key" :label="t('common.key')">
          <el-input v-model="model.key" :disabled="!isNew" data-cy="key" />
        </el-form-item>

        <el-form-item prop="value" :label="t('common.value')">
          <el-input
            v-model="model.value"
            type="textarea"
            autosize
            data-cy="value"
          />
        </el-form-item>
      </el-form>
    </div>

    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'keyValue',
          action: 'edit',
        }"
        @confirm="save"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
