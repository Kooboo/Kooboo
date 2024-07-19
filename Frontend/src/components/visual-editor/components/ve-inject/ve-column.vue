<template>
  <div ref="root" class="ve-column-root" @click.stop="onColumnClick">
    <VueDraggable
      class="ve-column-container"
      :class="{ 've-empty': !list.length }"
      :data-placeholder="t('ve.dropContentBlocksHere')"
      :list="(list as unknown[])"
      group="ve"
      item-key="id"
      :set-data="setData"
      @change="onListChanged"
      @end="updateSelect"
    >
      <template #item="{ element }">
        <div
          :data-content-after="element.name"
          :data-id="element.id"
          data-type="wrapper"
          @click.stop="onWidgetClick(element, $event)"
          @mouseenter.stop="onHolderWidget(element, $event)"
          @mouseout.stop="onHolderWidget(null, $event)"
          @dragenter.stop="setDragToMeta(element)"
        >
          <div :key="element.id" data-type="element" v-html="element.htmlStr" />
        </div>
      </template>
    </VueDraggable>
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import {
  ref,
  onMounted,
  toRaw,
  computed,
  watch,
  onBeforeUnmount,
  nextTick,
} from "vue";
import { on } from "@/utils/dom";
import type { MessageIframe, InUpdateDomContextType } from "../../types";
import VueDraggable from "vuedraggable";
import { cloneDeep, debounce } from "lodash-es";

import type { Meta } from "../../types";
import { beforeDrop } from "./hooks";
import { useInjectGlobalStore } from "./inject-global-store";
import { isColumn, isRow } from "../../utils/widget";
import { postParentMessage } from "./message";

const { t } = useI18n();

const list = ref<Meta[]>([]);
const props = defineProps<{
  kPlaceholder?: string;
  modelValue: Meta;
  row: Meta;
  column: Meta;
}>();
const emits = defineEmits<{
  (e: "update:modelValue", value: Meta): void;
  (e: "add-row", value: Meta): void;
  (e: "update:row", value: Meta): void;
  (e: "update:column", value: Meta): void;
}>();

const container = computed(() => props.kPlaceholder ?? "");

const root = ref();
const { selectWidget, hoverWidget, updateSelect, setDragToMeta } =
  useInjectGlobalStore();

function setData(e: DataTransfer, item: any) {
  const context = cloneDeep(item.__draggable_context.element);
  e.setData("application/json", JSON.stringify(context));
}

const onListChanged = debounce(function () {
  const column = cloneDeep(props.modelValue);
  column.children = cloneDeep(toRaw(list.value));
  emits("update:modelValue", column);
}, 500);

watch(() => list.value, onListChanged, {
  deep: true,
});

const resizeObserver = new ResizeObserver(() => {
  updateSelect();
});

onMounted(() => {
  list.value = props.modelValue?.children ?? [];

  const el = root.value;
  on(el, "dragover", (e) => {
    e.preventDefault(); // Allow the element to be dropped
    e.stopPropagation();
  });

  on(el, "drop", async (e) => {
    e.preventDefault();
    e.stopPropagation();

    const ctx = e as DragEvent;
    const json: Meta = JSON.parse(
      ctx.dataTransfer!.getData("application/json")
    );
    if (isRow(json)) {
      emits("add-row", json);
      return;
    }
    if (json.id) {
      // existing widgets
      return;
    }

    await beforeDrop(json);
    const targetId = (ctx.target as HTMLElement)?.getAttribute("data-id");
    if (targetId) {
      const ix = list.value.findIndex((it) => it.id === targetId);
      if (ix > -1) {
        list.value.splice(ix + 1, 0, json);
        return;
      }
    }
    list.value.push(json);
  });

  on(window, "message", async (e) => {
    const ctx = e as MessageEvent<MessageIframe<any>>;
    const data = ctx.data;
    if (!data.type?.startsWith("in-") || data.group !== container.value) {
      return;
    }

    if (data.type === "in-update-dom") {
      const { meta, context }: InUpdateDomContextType = data.context;
      if (isRow(meta) && meta.id === props.row?.id) {
        emits("update:row", cloneDeep(meta));
        return;
      }
      if (isColumn(meta) && meta.id === props.column?.id) {
        emits("update:modelValue", cloneDeep(meta));
        return;
      }
      const { rowId, columnId } = context ?? {};
      if (rowId !== props.row?.id || columnId !== props.column?.id) {
        // 不是当前行或列，跳过
        return;
      }

      list.value = list.value.map((it) => {
        if (it.id === data.context.meta.id) {
          return cloneDeep(meta);
        }
        return it;
      });

      nextTick(() => {
        const el: HTMLElement | null = document.querySelector(
          `[data-id="${meta.id}"]`
        );
        if (el) {
          selectWidget(meta, el, container.value, context);
        }
      });
    }
  });
  nextTick(() => {
    resizeObserver.observe(el);
  });
});

onBeforeUnmount(() => {
  resizeObserver.unobserve(root.value);
});

function onHolderWidget(item: Meta | null, e: MouseEvent | null) {
  const el = e?.target as HTMLElement;
  hoverWidget(item, el, container.value);
}

function onWidgetClick(item: Meta, e: MouseEvent) {
  const el = e.target as HTMLElement;

  if (!el?.getAttribute("data-id")) {
    return;
  }
  selectWidget(item, el, container.value, {
    id: item.id,
    rowId: props.row.id,
    columnId: props.column.id,
  });
}

function onColumnClick() {
  // 会进入这里说明Column里是空的
  postParentMessage("widgets", "switch-tab", container.value);
}
</script>

<style lang="scss" scoped>
@import "mixins.scss";
.ve-column-root {
  display: flex;
  width: 100%;
}
.ve-column-container {
  // min-height: 60px;
  height: 100%;
  width: 100%;
  position: relative;
  &.ve-empty {
    @include ve-empty($main-blue);
  }

  [data-type="wrapper"] {
    display: flex;
    width: 100%;
    position: relative;
    justify-content: center;
  }

  [data-type="element"] {
    pointer-events: none;
    width: 100%;
  }

  :deep([data-type="element"]) {
    * {
      box-sizing: border-box !important;
    }
  }
}
</style>
