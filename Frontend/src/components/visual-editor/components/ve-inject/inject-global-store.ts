import type {
  InSelectWidgetContextType,
  Meta,
  VeRenderContext,
  VeWidgetSelectContext,
  VeWidgetType,
} from "../../types";
import { cloneDeep, isEmpty } from "lodash-es";
import { computed, nextTick, ref } from "vue";
import { ensureSection, isRow } from "../../utils/widget";

import { emptyGuid } from "@/utils/guid";
import { postParentMessage } from "./message";

function defaultRootMeta(): Meta {
  return {
    children: [],
    htmlStr: "",
    id: emptyGuid,
    name: "PAGE",
    props: {},
    propDefines: [],
    type: "page",
  };
}

const rootKey = ref(0);
const renderContext = ref<VeRenderContext>({
  baseUrl: "/",
  classic: false,
});
const rootMeta = ref<Meta>(defaultRootMeta());
const metaParentMaps = ref<Record<string, Meta>>({});
const activeItem = ref<Meta>();
const activeId = computed<string>(() => activeItem.value?.id ?? "");
const activeContext = ref<VeWidgetSelectContext>();
const activeGroup = ref<string>();
const activeRect = ref<DOMRect>();

const dragToItem = ref<Meta>();
const dragFromItem = ref<VeWidgetType>();

const hoverGroup = ref<string>();
const hoverItem = ref<Meta>();
const hoverId = computed(() => hoverItem.value?.id ?? "");
const hoverRect = ref<DOMRect>();
const i18n = ref<Record<string, string>>({});

const activeItemTree = computed<Meta[]>(() => {
  const result: Meta[] = [];
  if (!activeItem.value) {
    return result;
  }

  if (isRow(activeItem.value)) {
    return result;
  }
  let parent: Meta = metaParentMaps.value[activeId.value];
  while (parent && parent.id) {
    result.push(parent);
    if (isRow(parent)) {
      break;
    }
    parent = metaParentMaps.value[parent.id];
  }

  return result;
});

function selectWidget(
  meta: Meta | null,
  el: HTMLElement | null,
  group: string,
  context?: VeWidgetSelectContext
) {
  activeGroup.value = group;
  activeItem.value = meta ?? undefined;
  activeContext.value = context;
  activeRect.value = el?.getBoundingClientRect();
  const data: InSelectWidgetContextType = {
    meta: meta ?? undefined,
    context,
    group,
  };
  postParentMessage(data, "select", group);
}

function hoverWidget(meta: Meta | null, el: HTMLElement | null, group: string) {
  hoverGroup.value = group;
  hoverItem.value = meta ?? undefined;
  hoverRect.value = el?.getBoundingClientRect();
}

/**
 * Reposition active element
 */
function updateSelect() {
  setDragToMeta();
  nextTick(() => {
    const el: HTMLElement | null = document.querySelector(
      `[data-id="${activeId.value}"]`
    );
    if (el) {
      selectWidget(
        activeItem.value ?? null,
        el,
        activeGroup.value ?? "",
        activeContext.value
      );
    }
  });
}

/**
 * Reposition hover element
 */
function updateHover() {
  setDragToMeta();
  nextTick(() => {
    const el: HTMLElement | null = document.querySelector(
      `[data-id="${hoverId.value}"]`
    );
    if (el) {
      hoverWidget(hoverItem.value ?? null, el, hoverGroup.value ?? "");
    }
  });
}

function resetHolder() {
  activeItem.value = undefined;
  hoverItem.value = undefined;
  selectWidget(null, null, "");
}

function assignParent(parent: Meta, maps: Record<string, Meta>) {
  for (const child of parent.children) {
    maps[child.id!] = parent;
    if (!isEmpty(child.children)) {
      assignParent(child, maps);
    }
  }
}

function resetMetaParentMaps() {
  const maps: Record<string, Meta> = {};
  assignParent(rootMeta.value, maps);
  metaParentMaps.value = maps;
}

function updateRootMeta(meta: Meta) {
  rootKey.value++; // 用来保证iframe里面的内容重新渲染
  const data = cloneDeep(meta);
  rootMeta.value = data;
  resetMetaParentMaps();
}

function getRowMeta(meta?: Meta) {
  if (!meta || isRow(meta)) {
    return meta;
  }
  return getRowMeta(metaParentMaps.value[meta.id!]);
}

function setDragToMeta(meta?: Meta) {
  if (!meta || !dragFromItem.value || !isRow(dragFromItem.value.meta)) {
    dragToItem.value = meta;
    return;
  }

  dragToItem.value = getRowMeta(meta);
}
function setDragFromMeta(widget?: VeWidgetType) {
  dragFromItem.value = widget;
}

function init(
  meta: Meta,
  options: {
    baseUrl: string;
    i18n: Record<string, string>;
    classic: boolean;
  }
) {
  const data = cloneDeep(meta);
  document.querySelectorAll("ve-placeholder").forEach((it) => {
    const section = it.getAttribute("k-placeholder") ?? "";
    ensureSection(data, section);
  });
  updateRootMeta(data);
  postParentMessage(data, "ready");

  renderContext.value = {
    baseUrl: options.baseUrl,
    classic: options.classic,
    rootMeta: data,
  };
  i18n.value = options.i18n;
}

function t(key: string) {
  return i18n.value[key] ?? key;
}

export function useInjectGlobalStore() {
  return {
    rootKey,
    rootMeta,
    renderContext,

    activeItem,
    activeItemTree,
    activeRect,
    activeGroup,
    activeContext,
    selectWidget,
    updateSelect,
    updateHover,

    hoverItem,
    hoverRect,
    hoverGroup,
    resetMetaParentMaps,
    hoverWidget,

    dragToItem,
    setDragToMeta,
    setDragFromMeta,

    resetHolder,
    updateRootMeta,
    init,
    t,
  };
}
