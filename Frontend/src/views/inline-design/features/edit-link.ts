import { addHistory, tryGetPureParent } from "@/views/inline-design/state";
import { editLink } from "@/views/inline-design/link/link-dialog";
import {
  domSources,
  getBinding,
  getKoobooBindings,
  getKoobooId,
  hasStringFormat,
  isDynamicAttribute,
  isDynamicContent,
  isEditableDomAttribute,
  isSelfClosed,
} from "../binding";
import type { AttributeChange, DomChange } from "../state/change";
import { getDynamicAttributeChange } from "../state/change";
import { getDynamicChange } from "../state/change";
import { ensurePlaceholderAttribute } from "../state";
import { i18n } from "@/modules/i18n";
import { createDomOperation } from "@/views/inline-design/state/operation";
import { clearKoobooMark } from "../utils/dom";
import { getElementParents, isTag } from "@/utils/dom";

const { t } = i18n.global;
export const name = "editLink";
export const display = t("common.link");
export const icon = "icon-link1";
export const order = 0;

export function active(el: HTMLElement) {
  if (!isTag(el, "a")) return;

  if (
    getElementParents(el, true).find((f) => f.tagName?.toLowerCase() == "svg")
  ) {
    return;
  }

  const pureParent = tryGetPureParent(el);
  if (!pureParent) return false;
  const bindings = getKoobooBindings(pureParent.element);
  if (isDynamicContent(bindings, pureParent.element))
    return !hasStringFormat(bindings);

  if (isDynamicAttribute(bindings, pureParent.element, "href")) {
    return !hasStringFormat(bindings, "href");
  }

  if (hasStringFormat(bindings, "href")) return false;

  if (
    bindings.find(
      (f) =>
        f.source === null && f.format == "attribute" && f.attribute == "href"
    )
  ) {
    return false;
  }

  return isEditableDomAttribute(bindings, el);
}

export async function invoke(el: HTMLElement) {
  const operation = await editAnchor(el);
  if (operation) addHistory([operation]);
}

export async function editAnchor(el: HTMLElement) {
  const { element, isSelf } = tryGetPureParent(el)!;
  const targetElement = isSelf ? el.parentElement! : element;
  const originContent = targetElement.innerHTML;

  try {
    await editLink(el as HTMLAnchorElement);
    if (originContent == targetElement.innerHTML) return;
    const id = ensurePlaceholderAttribute(targetElement);
    const operation = createDomOperation(id, originContent);
    const change = getChange(el);
    if (!change) throw new Error("can not get change");
    operation.changes.push(change);
    return operation;
  } catch (error) {
    el.parentElement!.innerHTML = originContent;
    throw new Error("edit link error");
  }
}

function getChange(el: HTMLElement) {
  const bindings = getKoobooBindings(el);
  if (isSelfClosed(bindings)) return false;
  const dynamicChange = getDynamicChange(bindings, el);
  if (dynamicChange) return dynamicChange;
  const dynamicAttributeChange = getDynamicAttributeChange(
    bindings,
    el,
    "href"
  );
  if (dynamicAttributeChange) return dynamicAttributeChange;

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
        attribute: "href",
        value: el.getAttribute("href"),
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
