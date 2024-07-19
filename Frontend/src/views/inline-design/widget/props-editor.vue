<template>
  <el-tabs v-model="currentTab">
    <el-tab-pane v-if="fields.length" :label="t('common.basic')" name="basic">
      <el-form
        :model="model"
        :rules="rules"
        label-position="top"
        class="props-editor-form"
        @submit.prevent
      >
        <template v-for="item in fields" :key="item.name">
          <component
            :is="getFieldControl(item)"
            :field="item"
            :model="model"
            :css-class="widgetClass(item)"
          />
        </template>
      </el-form>
    </el-tab-pane>
    <el-tab-pane
      v-if="containerFields.length"
      :label="t('ve.containerStyles')"
      name="container"
    >
      <el-form
        :model="model"
        :rules="rules"
        label-position="top"
        class="props-editor-form"
        @submit.prevent
      >
        <template v-for="item in containerFields" :key="item.name">
          <component
            :is="getFieldControl(item)"
            :field="item"
            :model="model"
            :css-class="widgetClass(item)"
          />
        </template>
      </el-form>
    </el-tab-pane>
  </el-tabs>
</template>

<script lang="ts" setup>
import {
  getFieldControl,
  usePropsEditor,
} from "@/components/visual-editor/components/ve-props-edit/effects";
import type { Meta } from "@/components/visual-editor/types";
import { onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { kebabCase, debounce } from "lodash-es";
import type { Field } from "@/components/field-control/types";
import { isClassic } from "@/components/visual-editor/utils";

const { t } = useI18n();

function widgetClass(item: Field) {
  return kebabCase(`ve-${item.controlType}`);
}

const props = defineProps<{
  modelValue: Meta;
}>();

const currentTab = ref<string>();

const classic = isClassic();
const { model, containerFields, fields, rules } = usePropsEditor(
  props.modelValue,
  classic
);

watch(
  () => model.value,
  debounce(function (data) {
    props.modelValue.props = data;
  }, 500),
  {
    deep: true,
  }
);
onMounted(() => {
  if (fields.value.length) {
    currentTab.value = "basic";
  } else if (containerFields.value.length) {
    currentTab.value = "container";
  } else {
    currentTab.value = undefined;
  }
});
</script>

<style lang="scss">
.props-editor-form {
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
