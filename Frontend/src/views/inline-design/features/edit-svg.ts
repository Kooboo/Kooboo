import { addHistory } from "@/views/inline-design/state";
import { editSvg } from "@/views/inline-design/svg";

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

const { t } = i18n.global;
export const name = "editSvg";
export const display = t("common.editSvg");
export const icon = "icon-a-writein";
export const order = 5;

export function active(el: HTMLElement) {
  el = getElementParents(el, true).find(
    (f) => f.tagName?.toLowerCase() == "svg"
  )!;
  if (!el) return false;
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
  el = getElementParents(el, true).find(
    (f) => f.tagName?.toLowerCase() == "svg"
  )!;

  const { element } = tryGetPureParent(el)!;
  const originContent = element.innerHTML;

  try {
    const svgString = await editSvg(el);
    const container = document.createElement("div");
    container.innerHTML = svgString;
    el.innerHTML = (container.firstChild as HTMLElement).innerHTML;
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

function getChange(el: HTMLElement) {
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
