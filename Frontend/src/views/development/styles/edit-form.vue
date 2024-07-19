<script lang="ts" setup>
import type { Style } from "@/api/style/types";
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
import { isUniqueName } from "@/api/style";
const props = defineProps<{ model: Style }>();
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
            isUniqueNameRule(isUniqueName, t("common.styleNameExistsTips")),
          ],
  } as Rules;
});

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <el-form ref="form" :model="model" :rules="rules" @submit.prevent>
    <el-form-item :label="t('common.styleName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-300px"
        :disabled="model.id !== emptyGuid"
        :title="model.name"
        data-cy="style-name"
      />
    </el-form-item>
  </el-form>
</template>
