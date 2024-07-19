import { addHistory, tryGetPureParent } from "@/views/inline-design/state";
import { showImageDialog } from "@/views/inline-design/image/image-editor";
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
import type { AttributeChange, Change, DomChange } from "../state/change";
import { getDynamicAttributeChange } from "../state/change";
import { getDynamicChange } from "../state/change";
import { ensurePlaceholderAttribute } from "../state";
import { i18n } from "@/modules/i18n";
import { createDomOperation } from "@/views/inline-design/state/operation";
import { getElementParents, isTag } from "@/utils/dom";
import { clearKoobooMark } from "../utils/dom";
import type { DomValueWrapper } from "@/global/types";

const { t } = i18n.global;
export const name = "editImage";
export const display = t("common.image");
export const icon = "icon-photo";
export const order = 0;

export function active(el: HTMLElement) {
  if (!isTag(el, "img")) return;
  const picture = getPictureParent(el);
  if (picture) el = picture;
  const pureParent = tryGetPureParent(el);
  if (!pureParent) return false;
  const bindings = getKoobooBindings(pureParent.element);
  if (isSelfClosed(bindings)) return false;

  if (isDynamicContent(bindings, pureParent.element)) {
    return !hasStringFormat(bindings);
  }

  if (hasStringFormat(bindings, "src")) return false;

  if (isDynamicAttribute(bindings, pureParent.element, "src")) {
    return !hasStringFormat(bindings, "src");
  }

  if (
    bindings.find(
      (f) =>
        f.source === null && f.format == "attribute" && f.attribute == "src"
    )
  ) {
    return false;
  }

  return isEditableDomAttribute(bindings, el);
}

export async function invoke(el: HTMLElement) {
  const operation = await editImage(el);
  if (operation) addHistory([operation]);
}

export async function editImage(el: HTMLElement) {
  const picture = getPictureParent(el);
  if (picture) el = picture;
  const { element, isSelf } = tryGetPureParent(el)!;
  const targetElement = isSelf ? el.parentElement! : element;
  const originContent = targetElement.innerHTML;

  try {
    const attributeChanges = await showImageDialog(el as HTMLImageElement);
    if (!attributeChanges.length) return;
    const id = ensurePlaceholderAttribute(targetElement);
    const operation = createDomOperation(id, originContent);
    const changes = getChanges(el, attributeChanges);
    if (!changes) throw new Error("can not get change");
    operation.changes.push(...changes);
    return operation;
  } catch (error) {
    el.parentElement!.innerHTML = originContent;
    throw new Error("edit image error");
  }
}

function getChanges(el: HTMLElement, attributeChanges: DomValueWrapper[]) {
  const bindings = getKoobooBindings(el);
  const dynamicChange = getDynamicChange(bindings);
  if (dynamicChange) return [dynamicChange];
  const domBinding = getBinding(bindings, domSources);
  if (!domBinding) return;
  const { element, isSelf } = tryGetPureParent(el, domBinding)!;
  const koobooId = getKoobooId(element)!;

  if (isSelf && !isTag(el, "picture")) {
    const changes: Change[] = [];

    for (const change of attributeChanges) {
      if (change.origin == change.value) continue;
      const dynamicAttributeChange = getDynamicAttributeChange(
        bindings,
        el,
        change.name!
      );

      if (dynamicAttributeChange) {
        changes.push(dynamicAttributeChange);
      } else {
        changes.push({
          source: domBinding.source,
          id: domBinding.id,
          action: "update",
          koobooId: koobooId,
          attribute: change.name,
          value: el.getAttribute(change.name!),
        } as AttributeChange);
      }
    }

    return changes;
  } else {
    return [
      {
        source: domBinding.source,
        id: domBinding.id,
        action: "update",
        koobooId: koobooId,
        value: clearKoobooMark(element.innerHTML),
      } as DomChange,
    ];
  }
}

function getPictureParent(el: HTMLElement) {
  const parents = getElementParents(el);
  const picture = parents.find((f) => isTag(f, "picture"));
  if (picture) return picture;
}
