<template>
  <div class="designer-root">
    <div
      :key="rootKey"
      ref="root"
      class="ve-root ve-global"
      @drop.stop="onRootDrop"
      @dragover.stop.prevent
      @dragenter.stop="setDragToMeta()"
      @click.stop="onRowClick"
    >
      <VueDraggable
        :key="rootMeta.id"
        class="ve-row"
        :class="{ 've-empty': !list.length }"
        :data-placeholder="t('ve.dropColumnBlocksHere')"
        :list="list"
        group="rows"
        item-key="id"
        :style="rootStyles"
        :set-data="setData"
        @change="onListChanged"
        @end="updateSelect"
      >
        <template #item="{ element, index }">
          <div
            :data-content-after="element.name"
            :data-id="element.id"
            data-type="wrapper"
            :style="getRowStyle(element)"
            @click.stop="onColumnClick(element, $event)"
            @mouseenter.stop="onHolderWidget(element, $event)"
            @mouseout.stop="onHolderWidget(null, null)"
          >
            <div
              v-for="(column, ix) in element.children"
              :key="column.id"
              class="ve-column-container"
              :style="getColumnStyle(column)"
              :data-id="column.id"
              @click.stop="onColumnClick(column, $event)"
              @mouseenter.stop="onHolderWidget(column, $event)"
              @mouseout.stop="onHolderWidget(null, null)"
            >
              <VeColumn
                v-model="element.children[ix]"
                v-model:row="list[index]"
                v-model:column="element.children[ix]"
                :k-placeholder="container"
                @add-row="onAdd($event, element)"
              />
            </div>
          </div>
        </template>
      </VueDraggable>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { ref, onMounted, toRaw, computed, watch } from "vue";
import { on } from "@/utils/dom";
import type { MessageIframe, VeDraggableElement } from "../../types";
import VueDraggable from "vuedraggable";
import VeColumn from "./ve-column.vue";
import { cloneDeep, isEmpty, debounce, omitBy } from "lodash-es";

import type { Meta } from "../../types";
import { useInjectGlobalStore } from "./inject-global-store";
import { createDefaultRow } from "../ve-columns/effects";
import { setContainerStyles } from "../../render/utils";
import { isRow, ensureSection, initMeta } from "../../utils/widget";
import { postParentMessage } from "./message";
import { getRootStyleObjects } from "../../page-styles";

const { t } = useI18n();

const props = defineProps<{
  kPlaceholder?: string;
}>();

const container = computed(() => props.kPlaceholder ?? "");
const rootStyles = computed(() => {
  const { body } = getRootStyleObjects(rootMeta.value.props ?? {}, false);
  return omitBy(body, (v, k) => k.startsWith("background-"));
});

const root = ref();
const {
  rootKey,
  rootMeta,
  selectWidget,
  hoverWidget,
  updateRootMeta,
  updateSelect,
  resetMetaParentMaps,
  setDragToMeta,
} = useInjectGlobalStore();
const list = ref<Meta[]>([]);

function init() {
  if (!list.value || isEmpty(list.value)) {
    const section = ensureSection(rootMeta.value, container.value);
    list.value = section.children;
  }
  if (isEmpty(list.value)) {
    list.value = createDefaultRow();
  }
  postParentMessage(toRaw(list.value), "init", container.value);
}

function setData(e: DataTransfer, item: VeDraggableElement) {
  const context = cloneDeep(item.__draggable_context.element);
  e.setData("application/json", JSON.stringify(context));
}

function getRowStyle(row: Meta): any {
  const el = document.createElement("div");
  setContainerStyles(el, row);
  return el.getAttribute("style");
}

function getColumnStyle(column: Meta): any {
  const el = document.createElement("div");
  el.style.setProperty("width", `${column.props["widthPercent"]}%`);
  setContainerStyles(el, column);
  return el.getAttribute("style");
}

function onListChanged() {
  rootMeta.value.children = rootMeta.value.children.map((section) => {
    if (section.name === container.value) {
      section.children = toRaw(list.value);
    }
    return toRaw(section);
  });

  postParentMessage(rootMeta.value, "sync");
  resetMetaParentMaps();
}

watch(
  () => list.value,
  debounce(function () {
    onListChanged();
  }, 500),
  {
    deep: true,
  }
);

function onAdd(meta: Meta, current?: Meta) {
  if (meta.id) {
    return;
  }
  initMeta(meta);
  if (isRow(meta)) {
    if (current) {
      const ix = list.value.findIndex((it) => it.id === current.id);
      list.value.splice(ix + 1, 0, meta);
    } else {
      list.value.push(meta);
    }
  } else {
    // TODO: 从底部/顶部放入
    // console.log(["add widget", meta, current]);
  }
}

onMounted(() => {
  on(window, "message", async (e) => {
    const ctx = e as MessageEvent<MessageIframe<any>>;
    const data = ctx.data;
    if (!data.type?.startsWith("in-") || data.group !== container.value) {
      return;
    }

    if (data.type === "in-sync") {
      updateRootMeta(data.context);
      list.value =
        rootMeta.value.children.find((it) => it.name === container.value)
          ?.children ?? createDefaultRow();
    }
  });
  init();
});

function onHolderWidget(item: Meta | null, e: MouseEvent | null) {
  const el = e?.target as HTMLElement;
  hoverWidget(item, el, container.value);
}

function onColumnClick(item: Meta, e: MouseEvent) {
  selectWidget(item, e.target as HTMLElement, container.value);
}

function onRowClick() {
  // 会进入这里说明Rows里是空的
  postParentMessage("columns", "switch-tab", container.value);
}

function onRootDrop(e: Event) {
  e.preventDefault();
  const ctx = e as DragEvent;
  const json: Meta = JSON.parse(ctx.dataTransfer!.getData("application/json"));
  onAdd(json);
}
</script>

<style lang="scss">
html,
body {
  min-height: 100%;
}
</style>

<style lang="scss" scoped>
@import "mixins.scss";

.designer-root {
  display: flex;
  flex-direction: column;
  width: 100%;
  margin-bottom: 30px; // 留出底部空白给最后一行的工具栏
  padding: 0;

  .ve-root {
    padding: 0;
    width: 100%;

    .ve-row {
      min-height: 60px;
      flex-direction: column;
      &.ve-empty {
        padding: 10px;
        @include ve-empty($main-orange);
      }
      [data-type="wrapper"] {
        display: flex;
        width: 100%;
        position: relative;
        justify-content: center;
        border: 2px dashed transparent;
        // border: 2px dashed #d1d1d1;
        cursor: move;
        .ve-column-container {
          border: 1px dashed transparent;
          // border: 1px dashed #d1d1d1;
          // min-height: 60px;
          padding: 10px;
        }
      }
    }
  }
}
</style>
