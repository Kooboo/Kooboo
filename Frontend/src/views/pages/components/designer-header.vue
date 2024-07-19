<template>
  <div class="pt-16 flex items-stretch justify-between">
    <el-form ref="form" :model="modelValue" :rules="rules">
      <el-form-item :label="t('common.pageName')" prop="name">
        <el-input
          v-model="modelValue.name"
          class="w-300px"
          :title="modelValue.name"
          :disabled="isEdit"
          data-cy="page-name"
          @change="nameChanged"
        />
      </el-form-item>
    </el-form>
    <div>
      <VeDevice />
    </div>
  </div>
</template>

<script lang="ts" setup>
import VeDevice from "@/components/visual-editor/components/ve-content/ve-device.vue";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Rules } from "async-validator";
import { isUniqueNameRule, rangeRule, requiredRule } from "@/utils/validate";
import { isUniqueName } from "@/api/pages";
import type { PostPage } from "@/api/pages/types";

const { t } = useI18n();
const form = ref();
const rules = computed(
  () =>
    ({
      name: props.isEdit
        ? []
        : [
            rangeRule(1, 50),
            requiredRule(t("common.nameRequiredTips")),
            isUniqueNameRule(
              isUniqueName,
              t("common.thePageNameAlreadyExists")
            ),
          ],
    } as Rules)
);

const props = defineProps<{
  modelValue: PostPage;
  isEdit: boolean;
}>();

const nameChanged = (value: string) => {
  const newValue = value.replace(/\s+/g, "") as string;
  if (!props.modelValue.urlPath) {
    props.modelValue.urlPath = `/${newValue}`;
  }
  props.modelValue.name = newValue;
};

async function validate() {
  await form.value?.validate();
}

defineExpose({
  validate,
});
</script>
