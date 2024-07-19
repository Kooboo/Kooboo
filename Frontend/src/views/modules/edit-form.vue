<script lang="ts" setup>
import {
  rangeRule,
  requiredRule,
  letterAndDigitStartRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { ref } from "vue";

import { useI18n } from "vue-i18n";
defineProps<{ model: { name: string } }>();
const { t } = useI18n();
const form = ref();

const rules = {
  name: [
    rangeRule(1, 50),
    requiredRule(t("common.nameRequiredTips")),
    letterAndDigitStartRule(),
  ],
} as Rules;

const emit = defineEmits<{
  (e: "save"): void;
}>();

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <el-form
    ref="form"
    label-position="top"
    :model="model"
    :rules="rules"
    @submit.prevent
  >
    <el-form-item :label="t('common.name')" prop="name">
      <el-input
        v-model="model.name"
        data-cy="name"
        @keydown.enter="emit('save')"
      />
    </el-form-item>
  </el-form>
</template>
