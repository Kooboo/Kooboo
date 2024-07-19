<script lang="ts" setup>
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable no-undef */

import { onMounted, onUnmounted, ref, watch } from "vue";
import { monaco } from "./userWorker";
import { newGuid } from "@/utils/guid";
import config from "./config";
import { dark } from "@/composables/dark";

const container = ref();
let originalModel: any = undefined;
let modifiedModel: any = undefined;
let editor: any = undefined;

const props = defineProps<{
  editable?: boolean;
  original: string;
  modified: string;
}>();

const emit = defineEmits<{
  (e: "update:modified", value: string): void;
}>();

onMounted(async () => {
  const originalModel = monaco.editor.createModel(
    props.original,
    "text/plain",
    monaco.Uri.file(newGuid())
  );

  const modifiedModel = monaco.editor.createModel(
    props.modified,
    "text/plain",
    monaco.Uri.file(newGuid())
  );

  if (props.editable) {
    modifiedModel.onDidChangeContent(() => {
      const content = modifiedModel.getValue();
      emit("update:modified", content);
    });
  }

  editor = monaco.editor.createDiffEditor(container.value, {
    enableSplitViewResizing: true,
    readOnly: !props.editable,
    ...config,
  });

  editor.setModel({
    original: originalModel,
    modified: modifiedModel,
  });
});

onUnmounted(() => {
  originalModel?.dispose();
  modifiedModel?.dispose();
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
