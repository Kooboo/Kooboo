import type { Meta, VeRenderContext, VeWidgetType } from "./types";
import { cloneDeep, isEmpty, orderBy } from "lodash-es";
import { findWidget, isCustomWidget } from "./utils/widget";

import type { VeWidgetSelectContext } from "./types";
import { emptyGuid } from "@/utils/guid";
import { preview } from "./render";
import { ref } from "vue";

function defaultRootMeta(): Meta {
  return {
    children: [],
    htmlStr: "",
    id: emptyGuid,
    name: "page",
    props: {},
    propDefines: [],
    type: "page",
  };
}

const renderContext = ref<VeRenderContext>({
  baseUrl: "/",
  classic: false,
});
const rootMeta = ref<Meta>(defaultRootMeta());
const customWidgets = ref<VeWidgetType[]>();

const activeWidget = ref<Meta>();
const activeGroup = ref("");
const activeContext = ref<VeWidgetSelectContext>();
const activeId = ref<string>();

async function updateWidgets(children: Meta[], renderContext: VeRenderContext) {
  if (isEmpty(children)) {
    return children;
  }
  const validWidgets: Meta[] = [];
  for (const child of children) {
    if (!isEmpty(child.children)) {
      child.children = await updateWidgets(child.children, renderContext);
    }
    if (!isCustomWidget(child)) {
      child.htmlStr = await preview(child, renderContext);
      validWidgets.push(child);
      continue;
    }
    const target = customWidgets.value?.find(
      (it) => it.meta.type === child.type && it.meta.name === child.name
    );
    if (!target) {
      continue;
    }
    child.propDefines = target.meta.propDefines;
    child.htmlStr = await preview(child, renderContext);
    validWidgets.push(child);
  }
  return validWidgets;
}

async function init(
  meta: Meta,
  ctx: {
    customWidgets?: VeWidgetType[];
    renderContext: VeRenderContext;
  }
) {
  meta = meta ?? defaultRootMeta();
  renderContext.value = {
    ...ctx.renderContext,
    rootMeta: meta,
  };
  const sortedWidgets = orderBy(
    cloneDeep(ctx.customWidgets ?? []),
    [
      (it: VeWidgetType) => it.meta?.type?.toLowerCase(),
      (it: VeWidgetType) => it.name.toLowerCase(),
    ],
    ["asc", "asc"]
  );
  customWidgets.value = sortedWidgets;
  meta.children = await updateWidgets(meta.children, renderContext.value);
  updateRootMeta(meta);
}

function updateRootMeta(meta: Meta) {
  rootMeta.value = cloneDeep(meta);
}
// selected
function selectWidget(
  item: Meta | null,
  group?: string,
  context?: VeWidgetSelectContext
) {
  activeWidget.value = item ?? undefined;
  activeGroup.value = group ?? "";
  activeId.value = item?.id;
  activeContext.value = context;
}

async function flushData(ctx: VeRenderContext) {
  const editing = activeWidget.value;
  if (editing) {
    const [target] = findWidget(rootMeta.value, (it) => it.id === editing.id);
    if (target) {
      target.props = editing.props;
      target.htmlStr = await preview(editing, ctx);
    }
  }
}

export function useGlobalStore() {
  return {
    renderContext,
    rootMeta,
    customWidgets,
    activeContext,
    activeWidget,
    activeGroup,
    activeId,
    flushData,
    init,
    updateRootMeta,
    selectWidget,
  };
}
