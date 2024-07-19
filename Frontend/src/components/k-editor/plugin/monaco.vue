<template>
  <div
    class="h-full border border-solid border-[#cccccc] focus-within:border-[#2296f3]"
  >
    <MonacoEditor ref="editor" v-model="sourceCode" language="html" />
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import type MonacoEditorType from "@/components/monaco-editor/index.vue";

import { sourceCode } from "./store";
const editor = ref<InstanceType<typeof MonacoEditorType>>();
// 光标置底
setTimeout(() => {
  const value = editor.value as any;
  value?.focus();
  value?.goToLine();
}, 500);
</script>
