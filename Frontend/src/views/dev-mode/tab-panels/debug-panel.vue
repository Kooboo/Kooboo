<script lang="ts" setup>
import type { PostCode, DebugSession } from "@/api/code/types";
import { getSession as getSessionApi, setBreakPoint } from "@/api/code";
import { useCodeStore } from "@/store/code";
import { onBeforeUnmount, ref, computed, watch } from "vue";
import { DEBUG_TAB_PREFIX } from "@/constants/constants";
import type { Action } from "@/store/dev-mode";
import { useDevModeStore } from "@/store/dev-mode";
import CtrlBar from "../components/debug/ctrl-bar.vue";
import Console from "../components/debug/console.vue";
import DebugEditor from "@/components/monaco-editor/debug.vue";
import JsonTree from "../components/debug/json-tree.vue";

const emit = defineEmits<{
  (e: "setAction", name: string, params: Partial<Action>): void;
}>();

const props = defineProps<{ id: string }>();
const codeStore = useCodeStore();
const devModeStore = useDevModeStore();
const model = ref<PostCode>();
const debugSession = ref<DebugSession>();
let active = true;
const currentCode = ref(props.id.replace(DEBUG_TAB_PREFIX, ""));

const getSession = async () => {
  if (!active) return;

  try {
    debugSession.value = await getSessionApi();
    if (debugSession.value.currentCode && !debugSession.value.end)
      await load(debugSession.value.currentCode);
  } catch (error) {
    console.log(error);
  }

  setTimeout(async () => {
    await getSession();
  }, 500);
};

const load = async (id?: string) => {
  if (id === currentCode.value) return;
  if (id) currentCode.value = id;
  if (devModeStore.debugTab) {
    model.value = await codeStore.getCode(currentCode.value);
    devModeStore.debugTab.name = model.value.name;
    emit("setAction", "open", { visible: !!model.value.url });
  }
};

const reload = async () => {
  model.value = await codeStore.getCode(currentCode.value);
};

const breakpoints = computed(() => {
  if (!debugSession.value) return [];
  return debugSession.value.breakpoints
    .filter((s) => s.source === currentCode.value)
    .map((m) => m.line);
});

const executingLine = computed(() => {
  if (debugSession.value?.end) return undefined;
  return debugSession.value?.debugInfo?.currentLine;
});

const checkBreakpoint = async (position: { line: number; column: number }) => {
  if (!debugSession.value) return;

  debugSession.value.breakpoints = await setBreakPoint({
    source: currentCode.value,
    line: position.line,
    column: position.column,
  });
};

onBeforeUnmount(() => {
  active = false;
});

load();
getSession();
defineExpose({ load, reload, currentCode });

const exception = ref();
const debugInfo = ref();

watch(
  () => debugSession.value,
  () => {
    exception.value = debugSession.value?.exception;
    debugInfo.value = debugSession.value?.debugInfo?.variables;
  },
  { deep: true }
);
</script>

<template>
  <div v-if="model" class="h-full flex bg-fff">
    <div class="flex-1 flex flex-col">
      <CtrlBar
        class="border-b border-solid border-line"
        :debug-session="debugSession"
      />
      <div class="flex-1 text-m font-mono relative">
        <div class="absolute inset-0">
          <DebugEditor
            v-model="model.body"
            :breakpoints="breakpoints"
            :executing="executingLine"
            @click="checkBreakpoint"
          />
        </div>
      </div>
      <div
        class="h-238px border-t border-solid border-line dark:bg-[#202428] p-4 text-s font-mono"
      >
        <Console :debug-session="debugSession" />
      </div>
    </div>

    <div
      v-if="exception || debugInfo"
      class="w-300px border-l border-solid border-line relative"
    >
      <ElScrollbar>
        <JsonTree v-if="exception" :data="exception" />
        <JsonTree v-else-if="debugInfo" :data="debugInfo" />
      </ElScrollbar>
    </div>
  </div>
</template>
