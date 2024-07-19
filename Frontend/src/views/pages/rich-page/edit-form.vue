<script lang="ts" setup>
import type { PostRichPage } from "@/api/pages/types";
import { isUniqueNameRule, rangeRule, requiredRule } from "@/utils/validate";
import { emptyGuid } from "@/utils/guid";
import { computed, ref } from "vue";

import { useI18n } from "vue-i18n";
import { isUniqueName, pageUrlIsUniqueName } from "@/api/pages";
const props = defineProps<{ model: PostRichPage; oldUrl: string }>();
const { t } = useI18n();
const form = ref();

const nameChanged = (value: string) => {
  if (!props.model) return;
  if (!props.model.url) {
    props.model.url = `/${value}`;
  }
};

const rules = computed((): any => {
  return {
    name:
      props.model.id !== emptyGuid
        ? []
        : [
            rangeRule(1, 50),
            requiredRule(t("common.nameRequiredTips")),
            isUniqueNameRule(
              isUniqueName,
              t("common.thePageNameAlreadyExists")
            ),
          ],

    url: [
      requiredRule(t("common.urlRequiredTips")),
      props.oldUrl === props.model.url
        ? ""
        : isUniqueNameRule(
            (name: string) => pageUrlIsUniqueName(name, props.oldUrl),
            t("common.urlOccupied")
          ),
    ],
    title: [requiredRule(t("common.titleRequiredTips"))],
  };
});

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
    <el-form-item :label="t('common.pageName')" prop="name">
      <el-input
        v-model="model.name"
        class="min-w-325px"
        :title="model.name"
        :disabled="model.id !== emptyGuid"
        data-cy="page-name"
        @input="model.name = model.name.replace(/\s+/g, '')"
        @change="nameChanged"
      />
    </el-form-item>
    <el-form-item label="URL" prop="url">
      <el-input
        v-model="model.url"
        class="min-w-325px"
        data-cy="url"
        @input="model.url = model.url.replace(/\s+/g, '')"
      />
    </el-form-item>
    <el-form-item :label="t('common.title')" prop="title">
      <el-input v-model="model.title" class="min-w-325px" data-cy="title" />
    </el-form-item>
    <slot />
    <el-form-item :label="t('page.online')">
      <el-switch v-model="model.published" />
    </el-form-item>
  </el-form>
</template>
