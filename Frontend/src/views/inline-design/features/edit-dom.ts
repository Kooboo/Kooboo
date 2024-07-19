import { addHistory } from "@/views/inline-design/state";
import { startEdit } from "@/views/inline-design/inline-editor";

import {
  getKoobooBindings,
  getKoobooId,
  domSources,
  getBinding,
  isEditableDomContent,
  isDynamicContent,
  hasStringFormat,
  isSelfClosed,
} from "../binding";
import type { DomChange } from "../state/change";
import { getDynamicChange } from "../state/change";
import { i18n } from "@/modules/i18n";

import {
  markDirty,
  ensurePlaceholderAttribute,
  tryGetPureParent,
} from "../state";
import { clearKoobooMark } from "../utils/dom";
import { createDomOperation } from "@/views/inline-design/state/operation";
import { getElementParents } from "@/utils/dom";
import { doc } from "@/views/inline-design/page";

const { t } = i18n.global;
export const name = "editDom";
export const display = t("common.edit");
export const icon = "icon-a-writein";
export const order = 0;

export const ignoreTags = [
  "img",
  "input",
  "video",
  "audio",
  "table",
  "col",
  "colgroup",
  "tbody",
  "td",
  "tfoot",
  "th",
  "thead",
  "tr",
  "canvas",
  "meter",
  "progress",
  "select",
  "textarea",
];

export function active(el: HTMLElement) {
  if (ignoreTags.includes(el.tagName?.toLowerCase())) return false;

  if (
    getElementParents(el, true).find((f) => f?.tagName?.toLowerCase() == "svg")
  ) {
    return false;
  }

  const pureParent = tryGetPureParent(el);
  if (!pureParent) return false;
  const bindings = getKoobooBindings(pureParent.element);
  if (isSelfClosed(bindings)) return false;
  if (bindings.find((f) => f.format == "text")) return false;
  if (isDynamicContent(bindings)) return !hasStringFormat(bindings);
  if (hasStringFormat(bindings)) return false;

  if (bindings.find((f) => f.source === null && f.format == "html")) {
    return false;
  }

  return isEditableDomContent(bindings, el);
}

export async function invoke(el: HTMLElement) {
  cleanSelectEvent(el);
  const { element } = tryGetPureParent(el)!;
  const originContent = element.innerHTML;

  try {
    await startEdit(el);
    if (originContent == el.innerHTML) return;
    const id = ensurePlaceholderAttribute(element);
    const operation = createDomOperation(id, originContent);
    const change = getChange(el);
    if (!change) throw new Error("can not get change");
    operation.changes.push(change);
    addHistory([operation]);
  } catch (error) {
    element.innerHTML = originContent;
  }
}

export function getChange(el: HTMLElement) {
  const bindings = getKoobooBindings(el);
  const dynamicChange = getDynamicChange(bindings);
  if (dynamicChange) return dynamicChange;
  const domBinding = getBinding(bindings, domSources);
  if (!domBinding) return;
  const { element } = tryGetPureParent(el, domBinding)!;
  const koobooId = getKoobooId(element)!;
  markDirty(element);

  return {
    source: domBinding.source,
    id: domBinding.id,
    action: "update",
    koobooId: koobooId,
    value: clearKoobooMark(element.innerHTML),
  } as DomChange;
}

function cleanSelectEvent(el: HTMLElement) {
  doc.value.onselectstart = null;
  doc.value.documentElement.style.userSelect = "all";
  el.onselectstart = null;
  el.style.userSelect = "all";
}
