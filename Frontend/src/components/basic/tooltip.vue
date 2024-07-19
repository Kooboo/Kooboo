<script lang="ts" setup>
import { computed } from "vue";
const props = defineProps<{
  tip: string;
  customClass?: string;
  icon?: string;
  enableHtml?: boolean;
}>();
const toolTipClass = computed(() => {
  return (props.icon ?? "icon-problem") + " " + props.customClass;
});
</script>

<template>
  <el-tooltip
    :content="tip"
    placement="top"
    popper-class="max-w-504px"
    :raw-content="enableHtml ? true : undefined"
  >
    <el-icon class="iconfont" :class="toolTipClass" @click.prevent />
  </el-tooltip>
</template>
