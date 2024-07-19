import { addHistory } from "@/views/inline-design/state";
import { editRepeater } from "@/views/inline-design/repeater";

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
import { getChildElements, getElementParents } from "@/utils/dom";
import { ignoreTags } from "./edit-dom";

const { t } = i18n.global;
export const name = "editRepeater";
export const display = t("common.editRepeater");
export const icon = "icon-view";
export const order = 20;

export function active(el: HTMLElement) {
  const repeatItemsParent = getRepeatItemsParent(el);
  if (!repeatItemsParent) return false;
  el = repeatItemsParent;

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
  const repeatItemsParent = getRepeatItemsParent(el)!;
  el = repeatItemsParent;

  const { element } = tryGetPureParent(el)!;
  const originContent = element.innerHTML;

  try {
    await editRepeater(el);
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

function getRepeatItemsParent(el: HTMLElement) {
  const parents = getElementParents(el);
  for (const parent of parents) {
    const children = getChildElements(parent);
    if (children.length < 2) continue;
    if ([...new Set(children.map(getElementStructure))].length > 1) continue;
    return parent;
  }
}

function getElementStructure(el: HTMLElement) {
  const children = [el, ...getChildElements(el)];
  return children.map((m) => m.tagName?.toLowerCase())?.join("_");
}
