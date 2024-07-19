import { cloneDeep, isEmpty } from "lodash-es";

import type { Meta } from "../types";
import { createDefaultRow } from "../components/ve-columns/effects";
import { getContainerPropDefines } from "./prop-define";
import { newGuid } from "@/utils/guid";

export function cloneMeta(row: Meta): Meta {
  const copiedRow = cloneDeep(row);
  copiedRow.id = newGuid();
  if (!isEmpty(copiedRow.children)) {
    copiedRow.children = copiedRow.children.map(cloneMeta);
  }
  return copiedRow;
}

export function isRow(meta: Meta) {
  return meta.type === "row";
}

export function isColumn(meta: Meta) {
  return meta.type === "column";
}

const customTypes = [
  "Form",
  "HtmlBlock",
  "Menu",
  "Module",
  "Script",
  "View",
  "Layout",
];

export function isCustomWidget(meta: Meta) {
  return customTypes.includes(meta.type);
}

export function ensureSection(
  rootMeta: Meta,
  name: string,
  rows?: Meta[]
): Meta {
  let section: Meta | undefined = rootMeta.children.find(
    (it) => it.name === name
  );
  if (!section) {
    section = {
      children: rows ?? createDefaultRow(),
      htmlStr: "",
      id: newGuid(),
      name,
      props: {},
      propDefines: [],
      type: "section",
    };
    rootMeta.children.push(section);
  }
  return section;
}

export function findWidget(
  meta: Meta,
  isMatch: (meta: Meta) => boolean,
  parent?: Meta
): [Meta | null, Meta | null] {
  if (isMatch(meta)) {
    return [meta, parent ?? null];
  }
  if (!isEmpty(meta.children)) {
    for (const child of meta.children) {
      const target = findWidget(child, isMatch, meta);
      if (target[0]) {
        return target;
      }
    }
  }
  return [null, null];
}

export function initMeta(meta: Meta) {
  meta.id = newGuid();
  const props: Record<string, any> = {};
  const containerProps = getContainerPropDefines(meta);
  containerProps.forEach((it) => {
    props[it.name] = it.defaultValue;
  });
  meta.propDefines.forEach((it) => {
    props[it.name] = it.settings["defaultValue"] ?? it.defaultValue;
  });
  meta.props = props;
  if (!isEmpty(meta.children)) {
    meta.children.forEach((it) => initMeta(it));
  }
}
