<script lang="ts" setup>
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable no-undef */

import { onMounted, onUnmounted, ref, watch } from "vue";
import { monaco } from "./userWorker";
import config from "./config";
import { newGuid } from "@/utils/guid";
import { createDecorations } from "./decoration";
import { dark } from "@/composables/dark";
import type { editor } from "./userWorker";

const container = ref();
const modelId = newGuid();
let model: editor.IModel;
let codeEditor: editor.IStandaloneCodeEditor;
let oldDecorations: any;
let hoverLine = ref<number>();

const props = defineProps<{
  modelValue: string;
  options?: Record<string, unknown>;
  breakpoints: number[];
  executing?: number;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: string): void;
  (e: "click", position: { line: number; column: number }): void;
}>();

watch(
  () => props.modelValue,
  () => {
    if (model && model.getValue() !== props.modelValue) {
      model.setValue(props.modelValue ?? "");
    }
  }
);

const setDecorations = () => {
  if (!codeEditor) return;
  const lineCount = model.getLineCount();
  const breakpoints = props.breakpoints.filter((f) => f <= lineCount);
  oldDecorations = codeEditor.deltaDecorations(
    oldDecorations || [],
    createDecorations(breakpoints, props.executing, hoverLine.value)
  );
};

watch(
  [
    () => props.breakpoints.toString(),
    () => props.modelValue,
    hoverLine,
    () => props.executing,
  ],
  setDecorations
);

onMounted(async () => {
  model = monaco.editor.createModel(
    props.modelValue,
    "typescript", //dot use javascript ,javascript will break code k
    monaco.Uri.file(modelId)
  );

  codeEditor = monaco.editor.create(container.value, {
    ...config,
    ...props.options,
    model,
    glyphMargin: true,
    readOnly: true,
    minimap: { enabled: false },
  });

  codeEditor.onMouseDown((e: any) => {
    if (e?.target?.detail?.glyphMarginLeft === undefined) return;
    if (e?.target?.detail?.isAfterLines) return;
    const line = e.target.position.lineNumber;
    const column = codeEditor!
      .getModel()!
      .getLineFirstNonWhitespaceColumn(line);
    emit("click", { line, column });
  });

  codeEditor.onMouseMove((e: any) => {
    hoverLine.value = e?.target?.position?.lineNumber;
  });

  setDecorations();
});

onUnmounted(() => {
  model?.dispose();
});

watch(dark, (dark) => monaco.editor.setTheme(dark ? "vs-dark" : "vs"), {
  immediate: true,
});
</script>

<template>
  <div
    ref="container"
    class="w-full h-full min-h-300px min-w-400px monaco-editor bg-fff"
  />
</template>

<style lang="scss">
.kooboo-debug-breakpoint {
  @apply !bg-orange rounded-full transform scale-75;
}

.kooboo-debug-executing {
  @apply bg-[rgba(255,230,0,0.3)];
}

.kooboo-debug-hover {
  @apply hover: bg-orange/50 rounded-full transform scale-75;
}
</style>
