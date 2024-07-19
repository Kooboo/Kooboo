<template>
  <PropHeader />
  <CustomWidgetPropEditor
    v-if="model"
    ref="form"
    :meta="model"
    :render-context="renderContext"
    @changed="onPropChanged"
  />
</template>

<script lang="ts" setup>
import type { Meta, VeRenderContext } from "../../types";
import CustomWidgetPropEditor from "./details.vue";
import { postInUpdateDomMessage } from "../../utils/message";
import type { VeWidgetSelectContext } from "../../types";
import PropHeader from "./prop-header.vue";
import { ref } from "vue";

const props = defineProps<{
  model?: Meta;
  group?: string;
  context?: VeWidgetSelectContext;
  renderContext: VeRenderContext;
}>();

const form = ref();

function onPropChanged() {
  if (!props.model) {
    return;
  }

  postInUpdateDomMessage({
    meta: props.model,
    group: props.group,
    context: props.context,
  });
}

async function validate() {
  await form.value?.validate();
}

defineExpose({
  validate,
});
</script>
