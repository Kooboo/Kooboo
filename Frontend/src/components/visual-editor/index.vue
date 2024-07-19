<template>
  <div class="flex w-full h-full">
    <div class="flex-1 pr-32 pl-80px flex flex-col min-h-400px min-w-600px">
      <slot name="header" />
      <div
        class="flex-1 rounded-normal overflow-hidden shadow-s-10 mb-100px bg-fff dark:bg-[#333] min-h-400px"
      >
        <template v-if="html">
          <VeContent
            :content="html"
            :loaded="loaded"
            :size="size"
            :page-styles="pageStyles"
            :render-context="renderContext"
          />
        </template>
      </div>
    </div>
    <div class="bg-card dark:bg-[#333] shadow-s-10 w-400px h-full">
      <VeSidebar
        ref="sidebarRef"
        :custom-tabs="customTabs ?? []"
        :render-context="renderContext"
      >
        <template v-for="item of customTabs" :key="item.key" #[item.key]>
          <slot :name="item.key" />
        </template>
      </VeSidebar>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {
  ref,
  watch,
  nextTick,
  provide,
  onMounted,
  onUnmounted,
  computed,
} from "vue";
import VeContent from "./components/ve-content/index.vue";
import VeSidebar from "./components/ve-sidebar/index.vue";

import { useGlobalStore } from "./global-store";
import type { Meta, VeWidgetType, DeviceType, VeRenderContext } from "./types";
import { renderPage } from "./renderer";
import type { KeyValue } from "@/global/types";
import {
  startHandleMessages,
  disposeMessages,
  handleMessage,
} from "./utils/message";
import { cloneDeep } from "lodash-es";

const props = defineProps<{
  modelValue: Meta;
  html: string;
  baseUrl: string;
  customTabs?: KeyValue[];
  customWidgets?: VeWidgetType[];
  pageStyles?: string;
  size?: DeviceType;
  pages?: KeyValue[];
  classic: boolean;
}>();

const { init, rootMeta, flushData } = useGlobalStore();

const form = ref();
const sidebarRef = ref();
const loaded = ref(false);

const universalBaseUrl = computed<string>(() => {
  const url = new URL(props.baseUrl);
  url.protocol = location.protocol;
  return url.toString();
});

const renderContext = computed<VeRenderContext>(() => {
  return {
    baseUrl: universalBaseUrl.value,
    classic: props.classic ?? false,
    rootMeta: rootMeta.value,
  };
});

function getPages(): KeyValue[] {
  return props.pages ?? [];
}

provide("ve-get-pages", getPages);
provide("is-classic", props.classic);

const emit = defineEmits<{
  (e: "update:modelValue", value: Meta): void;
  (e: "ready", initMeta: Meta): void;
}>();

async function loadDesign() {
  await init(props.modelValue, {
    customWidgets: props.customWidgets,
    renderContext: renderContext.value,
  });
  loaded.value = true;
}

watch(
  () => rootMeta.value,
  (v) => {
    emit("update:modelValue", v);
  },
  {
    deep: true,
  }
);

async function validate() {
  await flushData(renderContext.value);
  await sidebarRef.value?.validate();
  await form.value?.validate();
  emit("update:modelValue", cloneDeep(rootMeta.value));
}

handleMessage({
  ready(data) {
    rootMeta.value.children = data.item.children;
    emit("ready", rootMeta.value);
  },
});

onMounted(() => {
  startHandleMessages();
});

onUnmounted(() => {
  disposeMessages();
});

defineExpose({
  validate,
  getHtml(): Promise<Record<string, string>> {
    return new Promise((resolve) => {
      nextTick(async () => {
        const result = await renderPage(rootMeta.value, props.classic);
        resolve(result);
      });
    });
  },
  loadDesign,
});
</script>
