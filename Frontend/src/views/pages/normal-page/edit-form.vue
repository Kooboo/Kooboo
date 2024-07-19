<script lang="ts" setup>
import type { PostPage } from "@/api/pages/types";
import { isUniqueNameRule, rangeRule, requiredRule } from "@/utils/validate";
import { emptyGuid } from "@/utils/guid";
import { computed, ref } from "vue";

import { useI18n } from "vue-i18n";
import { isUniqueName, pageUrlIsUniqueName } from "@/api/pages";
const props = defineProps<{ model: PostPage; oldUrlPath?: string }>();
const { t } = useI18n();
const form = ref();

const nameChanged = (value: string) => {
  if (!props.model) return;
  if (!props.model.urlPath) {
    props.model.urlPath = `/${value}`;
  }
};
const rules = computed((): any => {
  return {
    name: [
      props.model.id === emptyGuid
        ? [
            rangeRule(1, 50),
            requiredRule(t("common.nameRequiredTips")),
            isUniqueNameRule(
              isUniqueName,
              t("common.thePageNameAlreadyExists")
            ),
          ]
        : [],
    ],
    urlPath: [
      requiredRule(t("common.urlRequiredTips")),
      props.oldUrlPath === props.model.urlPath
        ? ""
        : isUniqueNameRule(
            (name: string) => pageUrlIsUniqueName(name, props.oldUrlPath),
            t("common.urlOccupied")
          ),
    ],
  };
});

defineExpose({ validate: () => form.value?.validate() });
</script>

<template>
  <el-form ref="form" :model="model" :rules="rules" @submit.prevent>
    <el-form-item :label="t('common.pageName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-300px"
        :disabled="model.id !== emptyGuid"
        :title="model.name"
        data-cy="page-name"
        @input="model.name = model.name.replace(/\s+/g, '')"
        @change="nameChanged"
      />
    </el-form-item>
    <el-form-item label="URL" prop="urlPath">
      <el-input
        v-model="model.urlPath"
        class="min-w-300px"
        data-cy="url"
        @input="model.urlPath = model.urlPath.replace(/\s+/g, '')"
      />
    </el-form-item>
  </el-form>
</template>
