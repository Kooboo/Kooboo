<script lang="ts" setup>
import { isUniqueName } from "@/api/view";
import type { PostView } from "@/api/view/types";
import { emptyGuid } from "@/utils/guid";
import {
  isUniqueNameRule,
  rangeRule,
  requiredRule,
  simpleNameRule,
} from "@/utils/validate";
import { computed } from "@vue/reactivity";
import type { Rules } from "async-validator";
import { ref } from "vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{ model: PostView }>();
const { t } = useI18n();
const form = ref();

const rules = computed(() => {
  return {
    name:
      props.model.id !== emptyGuid
        ? []
        : [
            simpleNameRule(),
            rangeRule(1, 50),
            requiredRule(t("common.nameRequiredTips")),
            isUniqueNameRule(isUniqueName, t("common.viewNameExistsTips")),
          ],
  } as Rules;
});

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <el-form ref="form" :model="model" :rules="rules" @submit.prevent>
    <el-form-item :label="t('common.viewName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-300px"
        :disabled="model.id !== emptyGuid"
        :title="model.name"
        data-cy="view-name"
      />
    </el-form-item>
  </el-form>
</template>
