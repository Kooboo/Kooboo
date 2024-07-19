<template>
  <el-collapse v-model="group">
    <el-collapse-item
      v-if="fields.length"
      name="basic"
      :title="t('common.basic')"
    >
      <FieldsEditor
        ref="fieldsRef"
        :model="model"
        :fields="fields"
        :rules="rules"
      />
    </el-collapse-item>
    <el-collapse-item
      v-if="!['spacer'].includes(meta.type)"
      name="container"
      :title="t('ve.containerStyles')"
    >
      <FieldsEditor
        ref="containerRef"
        :model="model"
        :fields="containerFields"
        :rules="rules"
      />
    </el-collapse-item>
  </el-collapse>
</template>

<script lang="ts" setup>
import type { Meta, VeRenderContext } from "../../types";
import { ref, watch, onMounted } from "vue";
import { debounce, cloneDeep } from "lodash-es";
import { useI18n } from "vue-i18n";
import { usePropsEditor } from "./effects";
import FieldsEditor from "./ve-props-editor.vue";
import { useBuiltinWidgets } from "../ve-widgets/index";
import { preview } from "../../render";
import { isClassic } from "../../utils";

const { widgets } = useBuiltinWidgets();
const group = ref<string>();
const fieldsRef = ref();
const containerRef = ref();

const { t } = useI18n();

const emit = defineEmits<{
  (e: "changed", value: Record<string, string>): void;
}>();

const props = defineProps<{
  meta: Meta;
  renderContext: VeRenderContext;
}>();

const classic = isClassic();
const { model, containerFields, fields, rules } = usePropsEditor(
  props.meta,
  classic
);

onMounted(() => {
  if (fields.value.length) {
    group.value = "basic";
  } else if (containerFields.value.length) {
    group.value = "container";
  } else {
    group.value = undefined;
  }
});

(function () {
  const widget = widgets.value.find((it) => it.id === props.meta.type);
  if (typeof widget?.init === "function") {
    widget.init(model);
  }
})();

watch(
  () => model.value,
  debounce(async function (m: Record<string, any>) {
    let data = cloneDeep(m);
    props.meta.props = data;
    props.meta.htmlStr = await preview(props.meta, props.renderContext);
    emit("changed", data);
  }, 300),
  {
    deep: true,
  }
);

async function validate() {
  await fieldsRef.value?.validate();
  await containerRef.value?.validate();
  props.meta.props = cloneDeep(model.value);
}

defineExpose({
  validate,
});
</script>
