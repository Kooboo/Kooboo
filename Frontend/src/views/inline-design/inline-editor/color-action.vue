<script lang="ts" setup>
import type { Action } from "./types";
import { ref, watch } from "vue";
import { selectionChangeEvent, doc, win } from "../page";
import { getColorSelections } from "@/global/color";
import ColorPicker from "@/components/basic/color-picker.vue";

const props = defineProps<{ action: Action }>();
const color = ref();
const picker = ref();

const selections = getColorSelections(doc.value, win.value);

watch(
  () => selectionChangeEvent.value,
  () => {
    if (!props.action.active) return;
    color.value = props.action.active() ?? "#000";
  },
  { immediate: true, deep: true }
);
</script>

<template>
  <div
    class="text-black rounded-normal p-4 hover:bg-blue/10 flex items-center cursor-pointer relative"
  >
    <ColorPicker
      ref="picker"
      v-model="color"
      show-alpha
      :predefine="selections"
      @change="action.invoke($event)"
    >
      <div class="flex items-center space-x-4 h-24 px-4">
        <component
          :is="action.params.icon"
          v-if="action.params"
          class="w-16 h-16"
          :color="color"
        />
        <el-icon
          class="iconfont icon-pull-down text-s leading-none pointer-events-none"
        />
      </div>
    </ColorPicker>
  </div>
</template>
