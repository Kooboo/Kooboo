import { addHistory, tryGetPureParent } from "@/views/inline-design/state";
import { editStyle as showEditStyle } from "@/views/inline-design/style";
import {
  domSources,
  getKoobooBindings,
  getKoobooId,
  isEditableDomAttribute,
  getBinding,
  isDynamicContent,
  hasStringFormat,
  isSelfClosed,
} from "../binding";
import type { DomChange, AttributeChange } from "../state/change";
import { getDynamicChange } from "../state/change";
import { ensurePlaceholderAttribute } from "../state";
import { i18n } from "@/modules/i18n";
import { createDomOperation } from "@/views/inline-design/state/operation";
import { clearKoobooMark } from "../utils/dom";

const { t } = i18n.global;
export const name = "editStyle";
export const display = t("common.inlineStyle");
export const icon = "icon-style";
export const order = 0;

export function active(el: HTMLElement) {
  const pureParent = tryGetPureParent(el);
  if (!pureParent) return false;
  const bindings = getKoobooBindings(pureParent.element);
  if (isSelfClosed(bindings)) return false;
  if (isDynamicContent(bindings, pureParent.element))
    return !hasStringFormat(bindings, "style");
  return isEditableDomAttribute(bindings, el);
}

export async function invoke(el: HTMLElement) {
  const operation = await editStyle(el);
  if (operation) addHistory([operation]);
}

export async function editStyle(el: HTMLElement) {
  const { element, isSelf } = tryGetPureParent(el)!;
  const targetElement = isSelf ? el.parentElement! : element;
  const origin = targetElement.innerHTML;
  const parentInnerHtml = el.parentElement!.innerHTML;

  try {
    const styleChanges = await showEditStyle(el);
    if (origin == targetElement.innerHTML) return;
    if (!styleChanges.length) throw new Error("not change style");
    const id = ensurePlaceholderAttribute(el.parentElement!);
    const operation = createDomOperation(id, origin);
    const change = getChange(el);
    if (!change) throw new Error("can not get changes");
    operation.changes.push(change);
    return operation;
  } catch (error) {
    el.parentElement!.innerHTML = parentInnerHtml;
    throw new Error("change style error");
  }
}

export function getChange(el: HTMLElement) {
  const bindings = getKoobooBindings(el);
  const dynamicChange = getDynamicChange(bindings, el);
  if (dynamicChange) return dynamicChange;
  const domBinding = getBinding(bindings, domSources);

  if (domBinding) {
    const { element, isSelf } = tryGetPureParent(el, domBinding)!;
    const koobooId = getKoobooId(element)!;

    if (isSelf) {
      return {
        source: domBinding.source,
        id: domBinding.id,
        action: "update",
        koobooId: koobooId,
        attribute: "style",
        value: element.getAttribute("style")?.replaceAll('"', "'"),
      } as AttributeChange;
    } else {
      return {
        source: domBinding.source,
        id: domBinding.id,
        action: "update",
        koobooId: koobooId,
        value: clearKoobooMark(element.innerHTML),
      } as DomChange;
    }
  }
}
