import type { Meta, VeWidgetPropDefine, VeWidgetType } from "../../types";
import { createNumber, createPropDefine } from "../../utils/prop";

import { i18n } from "@/modules/i18n";
import { initMeta } from "../../utils/widget";
import { newGuid } from "@/utils/guid";
import { ref } from "vue";

type WidgetTypeData = Omit<
  VeWidgetType,
  "render" | "renderClassic" | "injection" | "init"
>;

const t = i18n.global.t;

function createColumn(
  name: string,
  widthPercent: number,
  cssClass: string
): Meta {
  const props: Record<string, any> = {};
  const propDefines: VeWidgetPropDefine[] = [
    createNumber("widthPercent", t("ve.widthPercent"), {
      defaultValue: widthPercent,
      isSystemField: true,
    }),
  ];
  if (cssClass) {
    createPropDefine("cssClass", {
      defaultValue: cssClass,
      isSystemField: true,
    });
  }
  const meta: Meta = {
    children: [],
    htmlStr: "",
    id: newGuid(),
    name,
    propDefines,
    props,
    type: "column",
  };
  initMeta(meta);
  return meta;
}

function createRow(name: string, widths: number[]): WidgetTypeData {
  const children = widths.map((w, ix) =>
    createColumn(`column${ix + 1}`, w, "")
  );
  const props: Record<string, any> = {};
  const meta: Meta = {
    children,
    htmlStr: "",
    id: newGuid(),
    name,
    propDefines: [],
    props,
    type: "row",
  };
  initMeta(meta);

  return {
    id: name,
    name,
    meta,
    settings: {},
    tooltip: "",
  };
}

const oneColumn = createRow(t("ve.oneColumn"), [100]);
const twoColumns11 = createRow(t("ve.twoColumns11"), [50, 50]);
const twoColumns12 = createRow(t("ve.twoColumns12"), [33.333, 66.666]);
const twoColumns13 = createRow(t("ve.twoColumns13"), [25, 75]);
const twoColumns21 = createRow(t("ve.twoColumns21"), [66.666, 33.333]);
const twoColumns31 = createRow(t("ve.twoColumns31"), [75, 25]);
const threeColumns = createRow(t("ve.threeColumns"), [33.333, 33.333, 33.333]);
const fourColumns = createRow(t("ve.fourColumns"), [25, 25, 25, 25]);

const widgets = ref<WidgetTypeData[]>([
  oneColumn,
  twoColumns11,
  twoColumns12,
  twoColumns13,
  twoColumns21,
  twoColumns31,
  threeColumns,
  fourColumns,
]);

export const createDefaultRow = () => [
  createRow(t("ve.oneColumn"), [100]).meta,
];

export function useColumns() {
  return {
    widgets,
  };
}
