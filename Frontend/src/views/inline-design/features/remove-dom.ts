import {
  domSources,
  getBinding,
  getKoobooBindings,
  getKoobooId,
  hasStringFormat,
  isDesignerWidget,
  isDynamicContent,
  isEditableDomAttribute,
  isSelfClosed,
} from "../binding";
import {
  ensurePlaceholderAttribute,
  markDirty,
  tryGetPureParent,
} from "../state";

import type { DomChange } from "../state/change";
import { K_REMOVE } from "../constants";
import { addHistory } from "@/views/inline-design/state";
import { clearKoobooMark } from "../utils/dom";
import { createDomOperation } from "@/views/inline-design/state/operation";
import { getChildElements } from "@/utils/dom";
import { getDynamicChange } from "../state/change";
import { i18n } from "@/modules/i18n";

const { t } = i18n.global;
export const name = "removeDom";
export const display = t("common.remove");
export const icon = "icon-delete";
export const order = 10;

export function active(el: HTMLElement) {
  if (isDesignerWidget(el)) {
    return false;
  }
  const pureParent = tryGetPureParent(el);
  if (!pureParent) return false;
  const bindings = getKoobooBindings(pureParent.element);
  if (isSelfClosed(bindings)) return false;
  if (isDynamicContent(bindings, pureParent.element))
    return !hasStringFormat(bindings);
  return isEditableDomAttribute(bindings, el);
}

export function invoke(el: HTMLElement) {
  const { element, isSelf } = tryGetPureParent(el)!;
  const targetElement = isSelf ? el.parentElement! : element;
  const originContent = targetElement.innerHTML;
  el.setAttribute(K_REMOVE, "true");
  const container = document.createElement("div");
  container.innerHTML = targetElement.innerHTML;

  getChildElements(container, true).forEach((f) => {
    if (f.hasAttribute(K_REMOVE)) {
      f.remove();
    }
  });

  const id = ensurePlaceholderAttribute(targetElement);
  const operation = createDomOperation(id, originContent);
  const change = getChange(el);
  if (!change) throw new Error("can not get change");
  el.remove();
  operation.changes.push(change);
  addHistory([operation]);
}

function getChange(el: HTMLElement) {
  const bindings = getKoobooBindings(el);
  const dynamicChange = getDynamicChange(bindings, el);
  if (dynamicChange) return dynamicChange;
  const domBinding = getBinding(bindings, domSources);

  if (domBinding) {
    const { element, isSelf } = tryGetPureParent(el, domBinding)!;
    const koobooId = getKoobooId(element)!;
    markDirty(element, isSelf ? "brothers" : "children");

    return {
      source: domBinding.source,
      id: domBinding.id,
      action: isSelf ? "delete" : "update",
      koobooId: koobooId,
      value: clearKoobooMark(element.innerHTML),
    } as DomChange;
  }
}
