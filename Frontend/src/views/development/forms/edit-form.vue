<script lang="ts" setup>
import type { PostForm } from "@/api/form/types";
import {
  isUniqueNameRule,
  simpleNameRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";
import type { Rules } from "async-validator";
import { computed, ref } from "vue";
import { emptyGuid } from "@/utils/guid";
import { useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import { isUniqueName } from "@/api/form";

const route = useRoute();
const props = defineProps<{ model: PostForm }>();
const { t } = useI18n();
const form = ref();

const validName = computed(() => {
  return (
    props.model.id && props.model.id === emptyGuid && !props.model?.isEmbedded
  );
});

const rules = computed(
  () =>
    ({
      name: validName.value
        ? [
            requiredRule(t("common.fieldRequiredTips")),
            rangeRule(1, 50),
            simpleNameRule(),
            isUniqueNameRule(isUniqueName, t("common.valueHasBeenTakenTips")),
          ]
        : [],
    } as Rules)
);

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <span
    v-if="route.query.type === 'embedded'"
    class="h-40px leading-10 dark:text-[#fffa]"
    >{{ t("common.embeddedForm") }}</span
  >
  <el-form v-else ref="form" :model="model" :rules="rules" @submit.prevent>
    <el-form-item :label="t('common.formName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-300px"
        :disabled="!validName"
        :title="model.name"
        data-cy="form-name"
      />
    </el-form-item>
  </el-form>
</template>
