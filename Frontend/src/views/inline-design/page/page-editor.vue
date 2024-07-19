<script lang="ts" setup>
import KFrame from "@/components/k-frame/index.vue";
import { useSiteStore } from "@/store/site";
import { computed, ref, triggerRef } from "vue";
import { editing } from "../inline-editor";
import { skipTags } from "../binding";
import { useI18n } from "vue-i18n";

import {
  selectionChangeEvent,
  currentElement,
  hoverElement,
  clickPosition,
  width,
  frame,
  doc,
  win,
  offset,
  leaveTip,
} from "@/views/inline-design/page";

import { historyCount, saveChanges } from "../state";
import { showConfirm } from "@/components/basic/confirm";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
const siteStore = useSiteStore();
const reloadUrl = location.href;
defineProps<{ content: string }>();

const focusElement = (e: MouseEvent) => {
  if (editing.value) return;
  const el = getPointElement(e);
  if (!el) return;
  currentElement.value = el;

  clickPosition.value = {
    x: e.x,
    y: e.y,
  };
};

const maskState = ref<boolean>(false);
const errorMask = ref<boolean>(false);
let cancellationToken: any;

function hiddenMask() {
  clearTimeout(cancellationToken);
  maskState.value = true;
  cancellationToken = setTimeout(() => {
    maskState.value = false;
  }, 20);
}

function changeHoverElement(e: PointerEvent) {
  const el = getPointElement(e);
  if (el) hoverElement.value = el;
}

function getPointElement(e: { x: number; y: number }) {
  const el = doc.value.elementFromPoint(
    e.x - offset.value.x,
    e.y - offset.value.y
  ) as HTMLElement;

  if (skipTags.includes(el?.tagName?.toLowerCase())) return;
  return el;
}

function frameUnload() {
  errorMask.value = true;
}

async function reload() {
  if (historyCount.value) {
    try {
      await showConfirm(t("common.reloadTip"));
      const path = getQueryString("path")!;
      await saveChanges(path);
    } catch (error) {
      //
    }
  }

  location.href = reloadUrl;
}

const agentEvents = (el: HTMLElement) => {
  const originAddEventListener = el.addEventListener;

  originAddEventListener("scroll", function () {
    triggerRef(currentElement);
    triggerRef(hoverElement);
  });

  originAddEventListener("selectionchange", function (e) {
    if (!editing.value) return;
    selectionChangeEvent.value = e;
  });

  win.value.onbeforeunload = () => {
    if (leaveTip.value) {
      return leaveTip.value;
    }
  };

  win.value.onunload = frameUnload;
};

const frameWidth = computed(() => {
  const size = width.value.size;
  if (!size) return size;
  return `${width.value.size}px !important`;
});
</script>

<template>
  <div
    v-loading="!content"
    :style="{ width: frameWidth }"
    class="h-full my-0 mx-auto"
  >
    <KFrame
      v-if="content && siteStore?.site?.baseUrl"
      :ref="(r:any) => (frame = r)"
      :content="content"
      :agent-events="agentEvents"
    />
    <div
      class="fixed inset-0"
      :style="{
        pointerEvents: maskState || editing ? 'none' : 'auto',
        cursor: hoverElement?.style.cursor,
      }"
      @pointermove="changeHoverElement"
      @wheel="hiddenMask"
      @click="focusElement"
    />
    <div v-if="errorMask" class="fixed inset-0 bg-gray z-100">
      <el-result
        icon="warning"
        :title="t('common.notEditableTitle')"
        :sub-title="t('common.notEditableTip')"
      >
        <template #extra>
          <el-button type="primary" @click="reload">{{
            t("common.refresh")
          }}</el-button>
        </template>
      </el-result>
    </div>
  </div>
</template>
