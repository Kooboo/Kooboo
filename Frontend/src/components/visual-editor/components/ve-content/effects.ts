import type {
  InSelectWidgetContextType,
  MessageIframe,
  MessageWidget,
  Meta,
  VeRenderContext,
  VeWidgetSelectContext,
} from "../../types";
import { cloneMeta, ensureSection, isColumn, isRow } from "../../utils/widget";
import { computed, ref, toRaw } from "vue";
import { handleMessage, postIFrameMessage } from "../../utils/message";

import { cloneDeep } from "lodash-es";
import { i18n } from "@/modules/i18n";
import { useBuiltinWidgets } from "../ve-widgets";
import { useGlobalStore } from "../../global-store";

const { widgets } = useBuiltinWidgets();

const { rootMeta, activeId, selectWidget, updateRootMeta } = useGlobalStore();

const { t } = i18n.global;

export type PageWidthType = {
  name: string;
  size?: number;
  display: string;
};

export const pageWidths = ref<PageWidthType[]>([
  {
    name: "full",
    size: undefined,
    display: t("common.fullScreen"),
  },
  {
    name: "pad",
    size: 820,
    display: t("common.pad"),
  },
  {
    name: "phone",
    size: 390,
    display: t("inlineDesign.phone"),
  },
]);

export const frame = ref();

export const doc = computed(
  () => frame.value?.element?.contentDocument as Document
);

export const win = computed(
  () => frame.value?.element?.contentWindow as Window
);

export function resetHolder() {
  selectWidget(null);
  const data: MessageIframe<string> = {
    type: "in-reset-holder",
    group: "",
    context: "",
  };
  postIFrameMessage(data);
}

export const width = ref<PageWidthType>(pageWidths.value[0]);

const getWidth = (name: string | null) =>
  pageWidths.value.find((f) => f.name == name) ?? pageWidths.value[0];

export const switchWidth = (name: string) => {
  width.value = getWidth(name);
  resetHolder();
};

type OnMetaFound = (ctx: {
  index: number;
  clonedItem: Meta;
  list: Meta[];
}) => void;

export function findMeta(
  rows: Meta[],
  item: Meta,
  ctx: VeWidgetSelectContext | undefined,
  onRowFound: OnMetaFound,
  onWidgetFound: OnMetaFound
) {
  if (isColumn(item)) {
    // column是预设的，不可操作
    return;
  }
  if (isRow(item)) {
    const targetIndex = rows.findIndex((it) => it.id === item.id);
    if (targetIndex > -1) {
      const copiedRow = cloneMeta(item);
      onRowFound({
        index: targetIndex,
        clonedItem: copiedRow,
        list: rows,
      });
    }
    return;
  }

  if (!ctx) {
    console.warn(["activeContext undefined"]);
    return;
  }
  if (!ctx.columnId) {
    console.warn(["column undefined", ctx]);
    return;
  }

  for (const row of rows) {
    if (row.id !== ctx.rowId) {
      continue;
    }
    for (const column of row.children) {
      if (column.id !== ctx.columnId) {
        continue;
      }

      const widgets = column.children;
      if (!widgets) {
        console.warn(["widgets is null", ctx]);
        return;
      }

      const targetIndex = widgets.findIndex((it) => it.id === item.id);
      if (targetIndex > -1) {
        const copiedItem = cloneMeta(item);
        onWidgetFound({
          index: targetIndex,
          clonedItem: copiedItem,
          list: widgets,
        });
      }
    }
  }
}

export function initIframeDesign(
  rootMeta: Meta,
  renderContext: VeRenderContext,
  pageStyles?: string
) {
  width.value = pageWidths.value[0];
  const i18n: Record<string, string> = {
    "common.copy": t("common.copy"),
    "common.delete": t("common.delete"),
    column: t("ve.column"),
    row: t("ve.row"),
    "ve.dragItHere": t("ve.dragItHere"),
  };
  widgets.value.forEach((it) => {
    i18n[it.id] = it.name;
  });
  frame.value.element.contentWindow.loadDesign(
    rootMeta,
    renderContext,
    pageStyles ?? "",
    i18n
  );
}

//-- iframe messages start
const _sync = (data: MessageWidget) => {
  updateRootMeta(data.item as Meta);
};
const _init = (data: MessageWidget) => {
  const section = ensureSection(rootMeta.value, data.group ?? "");
  section.children = data.item as Meta[];
};
const _select = (data: MessageWidget) => {
  const ctx: InSelectWidgetContextType = data.item;
  selectWidget(ctx.meta ?? null, ctx.group, ctx.context);
};
const _copy = (data: MessageWidget) => {
  const ctx: InSelectWidgetContextType = data.item;
  if (!ctx.meta) {
    return;
  }
  const group = ctx.group;
  const section = ensureSection(rootMeta.value, group || "");
  const rows = section.children;

  findMeta(
    rows,
    ctx.meta,
    ctx.context,
    ({ index, clonedItem, list }) => {
      list.splice(index + 1, 0, clonedItem);
      _changeSection(rows, group);
    },
    ({ index, clonedItem, list }) => {
      list.splice(index + 1, 0, clonedItem);
      _changeSection(rows, group);
    }
  );
};
const _delete = (data: MessageWidget) => {
  const ctx: InSelectWidgetContextType = data.item;
  if (!ctx.meta) {
    return;
  }
  const item: Meta = ctx.meta;

  const group = ctx.group;
  const section = ensureSection(rootMeta.value, group || "");
  const rows = section.children;

  findMeta(
    rows,
    ctx.meta,
    ctx.context,
    ({ index, list }) => {
      list.splice(index, 1);
      _changeSection(rows, group);
    },
    ({ index, list }) => {
      list.splice(index, 1);
      _changeSection(rows, group);
    }
  );

  if (item.id === activeId.value) {
    resetHolder();
  }
};

function _changeSection(rows: any[], group?: string) {
  const clonedMeta = cloneDeep(toRaw(rootMeta.value));
  const section = ensureSection(clonedMeta, group || "");
  section.children = rows;
  updateRootMeta(clonedMeta);

  const data: MessageIframe<Meta> = {
    type: "in-sync",
    group: group ?? "",
    context: clonedMeta,
  };
  postIFrameMessage(data);
}
export function handleIframeMessages() {
  handleMessage({
    sync: _sync,
    init: _init,
    select: _select,
    layout: resetHolder,
    copy: _copy,
    delete: _delete,
    ["switch-tab"]: resetHolder,
  });
}
//-- iframe messages end
