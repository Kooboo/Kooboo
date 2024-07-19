<script lang="ts" setup>
import { useSvgEditor } from "svg-editor-component";
import { ref, onMounted, watch } from "vue";
import { dark } from "@/composables/dark";
import { useI18n } from "vue-i18n";
import type { FormItemRule, ElForm, FormValidateCallback } from "element-plus";
import { simpleNameRule, requiredRule } from "@/utils/validate";

const props = defineProps<{
  svg?: string;
  name: string;
  url: string;
  siteUrl?: string;
  alt?: string;
  ext: string;
}>();
useSvgEditor("svg-editor-component");
const editor = ref();
const form = ref<InstanceType<typeof ElForm>>();
let isInit = false;
const { t } = useI18n();

interface EmitType {
  (e: "update:alt", params: string): void;
  (e: "update:name", params: string): void;
}
defineEmits<EmitType>();

onMounted(init);
const nameRules: FormItemRule[] = [
  requiredRule(t("common.nameRequiredTips")),
  simpleNameRule(),
];
function load() {
  if (editor.value) init();
}

function init() {
  try {
    if (isInit == true || !editor.value) return;
    editor.value.dark = dark.value;
    if (props.svg) editor.value.svgString = props.svg;
    isInit = true;
  } catch (e) {
    console.warn(e);
  }
}

watch(
  () => dark.value,
  (value: boolean) => {
    if (editor.value && isInit) editor.value.dark = value;
  }
);

watch(
  () => props.svg,
  (value) => {
    if (editor.value && isInit) {
      editor.value.svgString = value;
    }
  }
);

defineExpose({
  getSvgString() {
    return editor.value.svgString;
  },
  validate(cb?: FormValidateCallback) {
    form.value?.validate(cb);
  },
  resetFields: form.value?.resetFields,
});
</script>

<template>
  <div class="absolute inset-0 bottom-64 pb-64">
    <el-form
      ref="form"
      :model="{ name, alt }"
      class="file-info flex pt-16 px-32 space-x-16"
      @submit.prevent
    >
      <el-form-item
        :label="t('common.fileName')"
        prop="name"
        :rules="nameRules"
      >
        <el-input
          v-model="name"
          class="min-w-300px"
          :title="name"
          data-cy="fileName"
          @input="$emit('update:name', $event)"
        >
          <template v-if="ext" #append>{{ ext }}</template>
        </el-input>
      </el-form-item>
      <el-form-item :label="t('common.altText')" prop="alt">
        <el-input
          v-model="alt"
          class="min-w-300px"
          data-cy="alt"
          @input="$emit('update:alt', $event)"
        />
      </el-form-item>
      <el-form-item class="flex-1 ellipsis">
        <a
          class="w-full overflow-hidden ellipsis break-all text-[#999] hover:underline"
          :href="url"
          target="_blank"
          :title="t('common.preview')"
          data-cy="url"
        >
          {{ url }}
        </a>
      </el-form-item>
    </el-form>
    <svg-editor-component ref="editor" @load="load" />
  </div>
</template>

<style lang="scss" scoped>
:deep(.file-info) {
  .el-input {
    .el-input-group__append {
      background-color: var(--el-fill-color-light);
      padding: 0 20px;
    }
  }
}
</style>
