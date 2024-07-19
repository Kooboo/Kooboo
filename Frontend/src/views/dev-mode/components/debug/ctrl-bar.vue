<script lang="ts" setup>
import DebugBtn from "./debug-btn.vue";
import { useI18n } from "vue-i18n";
import type { DebugSession } from "@/api/code/types";
import { startSession, stepCode } from "@/api/code";
import { useShortcut } from "@/hooks/use-shortcuts";
import { useDevModeStore } from "@/store/dev-mode";
import { computed } from "@vue/reactivity";
import { DEBUG_TAB_PREFIX } from "@/constants/constants";

const props = defineProps<{ debugSession?: DebugSession }>();
const devModeStore = useDevModeStore();

const { t } = useI18n();

const debugTabActive = computed(() => {
  return devModeStore.activeTab?.id.startsWith(DEBUG_TAB_PREFIX);
});

useShortcut("StartDebug", () => {
  if (debugTabActive.value) startSession();
});
useShortcut("DebugContinue", () => {
  if (debugTabActive.value) stepCode("None");
});
useShortcut("DebugInto", () => {
  if (debugTabActive.value) stepCode("Into");
});
useShortcut("DebugOver", () => {
  if (debugTabActive.value) stepCode("Over");
});
useShortcut("DebugOut", () => {
  if (debugTabActive.value) stepCode("Out");
});
useShortcut("DebugStop", () => {
  if (debugTabActive.value) stepCode("Stop");
});

const finished = computed(() => !props.debugSession || props.debugSession?.end);
</script>

<template>
  <div
    class="py-4 px-8 flex items-center border-b border-solid border-line text-m dark:(bg-[#252526])"
  >
    <template v-if="finished">
      <DebugBtn
        icon="icon-play1"
        :display="t('common.start')"
        shortcut="F8"
        @click="startSession()"
      />
    </template>
    <template v-else>
      <DebugBtn
        icon="icon-play2"
        :display="t('common.continue')"
        shortcut="F5"
        @click="stepCode('None')"
      />

      <DebugBtn
        icon="icon-debug-step-into"
        :display="t('common.stepInto')"
        shortcut="F11"
        @click="stepCode('Into')"
      />

      <DebugBtn
        icon="icon-a-debug-step-over"
        :display="t('common.stepOver')"
        shortcut="F10"
        @click="stepCode('Over')"
      />

      <DebugBtn
        icon="icon-a-debug-step-out"
        :display="t('common.stepOut')"
        shortcut="Shift + F11"
        @click="stepCode('Out')"
      />
      <DebugBtn
        icon="icon-delete4"
        :display="t('common.stop')"
        shortcut="Shift + F5"
        @click="stepCode('stop')"
      />
    </template>
  </div>
</template>
