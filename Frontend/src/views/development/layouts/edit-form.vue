<script lang="ts" setup>
import { isUniqueName } from "@/api/layout";
import type { PostLayout } from "@/api/layout/types";
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
const props = defineProps<{ model: PostLayout; editMode: boolean }>();
const { t } = useI18n();
const form = ref();

const rules = computed(
  () =>
    ({
      name: props.editMode
        ? []
        : [
            rangeRule(1, 50),
            requiredRule(t("common.nameRequiredTips")),
            simpleNameRule(),
            isUniqueNameRule(isUniqueName, t("common.layoutNameExistsTips")),
          ],
    } as Rules)
);

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <el-form
    v-if="model"
    ref="form"
    :model="model"
    :rules="rules"
    @submit.prevent
  >
    <el-form-item :label="t('common.layoutName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-300px"
        :disabled="editMode"
        :title="model.name"
        data-cy="layout-name"
      />
    </el-form-item>
  </el-form>
</template>
