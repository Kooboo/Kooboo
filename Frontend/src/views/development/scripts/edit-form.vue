<script lang="ts" setup>
import { isUniqueName } from "@/api/script";
import type { Script } from "@/api/script/types";
import {
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  simpleNameRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { emptyGuid } from "@/utils/guid";
import { ref, computed } from "vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{ model: Script }>();
const { t } = useI18n();
const form = ref();

const rules = computed(() => {
  return {
    name:
      props.model.id !== emptyGuid
        ? ""
        : [
            rangeRule(1, 64),
            requiredRule(t("common.nameRequiredTips")),
            simpleNameRule(),
            isUniqueNameRule(isUniqueName, t("common.scriptNameExistsTips")),
          ],
  } as Rules;
});

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <el-form ref="form" :model="model" :rules="rules" @submit.prevent>
    <el-form-item :label="t('common.scriptName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-300px"
        :disabled="model.id !== emptyGuid"
        :title="model.name"
        data-cy="script-name"
      />
    </el-form-item>
  </el-form>
</template>
