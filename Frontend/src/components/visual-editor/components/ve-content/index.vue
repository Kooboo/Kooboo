<template>
  <div class="w-full h-full my-0 ve-content-wrapper">
    <div
      v-loading="loading || !loaded"
      class="mx-auto h-full"
      :style="{
        width: pageWidth,
        marginBottom: '20px',
        backgroundColor: '#ffffff',
      }"
    >
      <KFrame
        v-if="loaded && content && rootMeta"
        id="ve-iframe"
        :ref="(r: any) => (frame = r)"
        :base-url="renderContext.baseUrl"
        :content="content"
        height="500px"
        @load="onIframeLoad"
      />
    </div>
  </div>
</template>

<script lang="ts" setup>
import KFrame from "@/components/k-frame/index.vue";
import { computed, onMounted, ref, nextTick, watch } from "vue";
import {
  frame,
  width,
  handleIframeMessages,
  initIframeDesign,
  switchWidth,
} from "./effects";
import type { DeviceType, VeRenderContext } from "../../types";
import { useGlobalStore } from "../../global-store";
import { postIFrameMessage } from "../../utils/message";
const { rootMeta } = useGlobalStore();

const loading = ref(true);
const props = defineProps<{
  content: string;
  loaded: boolean;
  size?: DeviceType;
  pageStyles?: string;
  renderContext: VeRenderContext;
}>();

onMounted(() => {
  handleIframeMessages();
  if (props.size) {
    switchWidth(props.size);
  }
});

function onIframeLoad() {
  loading.value = false;
  nextTick(() => {
    initIframeDesign(rootMeta.value, props.renderContext, props.pageStyles);
  });
}

const pageWidth = computed(() => {
  if (!width.value.size) {
    return undefined;
  }

  return `${width.value.size}px`;
});

watch(
  () => props.pageStyles,
  (cssText) => {
    postIFrameMessage({
      type: "in-page-style",
      context: {
        meta: rootMeta.value,
        cssText,
      },
      group: "",
    });
  }
);
</script>

<style scoped lang="scss">
.ve-content-wrapper {
  background-color: rgb(255, 255, 255);
  background-image: linear-gradient(
      45deg,
      rgb(247, 247, 247) 25%,
      transparent 25%
    ),
    linear-gradient(-45deg, rgb(247, 247, 247) 25%, transparent 25%),
    linear-gradient(45deg, transparent 75%, rgb(247, 247, 247) 75%),
    linear-gradient(-45deg, transparent 75%, rgb(247, 247, 247) 75%);
  background-size: 20px 20px;
  background-position: 0px 0px, 0px 10px, 10px -10px, -10px 0px;
}
</style>
