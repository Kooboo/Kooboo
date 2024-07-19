<template>
  <el-tabs v-model="currentTab">
    <el-tab-pane
      v-if="generalFields.length"
      :label="t('common.general')"
      name="general"
    >
      <PropsEditor
        class="props-editor-form"
        :model="rootProps"
        :fields="generalFields"
      />
    </el-tab-pane>
    <el-tab-pane v-if="linkFields.length" :label="t('common.link')" name="link">
      <PropsEditor
        class="props-editor-form"
        :model="rootProps"
        :fields="linkFields"
      />
    </el-tab-pane>
  </el-tabs>
</template>

<script lang="ts" setup>
import type { Meta } from "@/components/visual-editor/types";
import { onMounted, ref, watch, toRaw } from "vue";
import { useI18n } from "vue-i18n";
import { kebabCase, debounce, cloneDeep } from "lodash-es";
import type { Field } from "@/components/field-control/types";
import { usePageStyles } from "@/components/visual-editor/page-styles";
import PropsEditor from "@/components/visual-editor/components/ve-props-edit/ve-props-editor.vue";
import { rootMeta } from ".";
const { rootProps, linkFields, generalFields, initPageStyles, getRootStyles } =
  usePageStyles();
const { t } = useI18n();

const props = defineProps<{
  modelValue: Meta;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: Meta): void;
  (e: "changed", value: Record<string, any>): void;
}>();

const currentTab = ref<string>();

onMounted(() => {
  initPageStyles(props.modelValue);
  if (generalFields.value.length) {
    currentTab.value = "general";
  } else if (linkFields.value.length) {
    currentTab.value = "link";
  } else {
    currentTab.value = undefined;
  }
});
</script>

<style lang="scss">
.props-editor-form .el-form {
  display: flex;
  flex-wrap: wrap;
  justify-content: flex-start;
  align-items: flex-start;
  .el-form-item {
    box-sizing: border-box;
    padding-right: 20px;
    width: 50%;
    &.w-full {
      width: 100%;
    }
    &.ve-rich-editor {
      width: 100%;
    }
    .el-form-item__content {
      > * {
        width: 100%;
      }
    }
  }
}
</style>
