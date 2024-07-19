<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { executeCode as executeCodeApi } from "@/api/code";
import type { DebugSession } from "@/api/code/types";
import JsonTree from "./json-tree.vue";

const props = defineProps<{ debugSession?: DebugSession }>();

const { t } = useI18n();
const consoleCode = ref("");
const codeResults = ref<unknown[]>([]);
const finished = computed(() => !props.debugSession || props.debugSession?.end);

const executeCode = async () => {
  if (!consoleCode.value) return;
  const result = await executeCodeApi(consoleCode.value);
  consoleCode.value = "";
  codeResults.value.unshift(result);
};

const tryParse = (content: string) => {
  try {
    return JSON.parse(content);
  } catch (error) {
    return content;
  }
};
</script>

<template>
  <div class="space-x-8 flex items-center dark:bg-[#202428]">
    <el-icon
      class="iconfont icon-a-nextstep2 dark:text-fff/86"
      :class="finished ? '' : ' animate-pulse animate-duration-1000'"
    />
    <input
      v-model="consoleCode"
      :disabled="!debugSession || debugSession?.end"
      class="outline-none w-full bg-transparent dark:text-fff/86"
      :class="finished ? 'cursor-not-allowed' : ''"
      :placeholder="t('common.inputCodeToExecuteTips')"
      @keydown.enter="executeCode"
    />
  </div>
  <ElScrollbar class="dark:bg-[#202428]">
    <div v-for="(item, index) of codeResults" :key="index">
      <JsonTree :data="tryParse(item as string)" />
    </div>
  </ElScrollbar>
</template>
