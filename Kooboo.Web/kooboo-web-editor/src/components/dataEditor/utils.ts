import { getParentElements } from "@/dom/utils";
import { isDynamicContent, getUnpollutedEl } from "@/kooboo/utils";
import { isEditable } from "../floatMenu/utils";
import { HOVER_BORDER_SKIP, KOOBOO_ID } from "@/common/constants";

export type editableData = { list: HTMLElement[]; parent: HTMLElement; koobooId: string };
export function getEditableData(element: HTMLElement): editableData | undefined {
  let els = getParentElements(element, true);
  if (els.length > 5) els = els.splice(0, 5);
  for (const el of els) {
    let element = getUnpollutedEl(el);
    if (!element) return;
    let parent = el == element ? element.parentElement : element;
    if (!parent || !isEditable(el) || isDynamicContent(parent)) continue;
    let koobooId = parent.getAttribute(KOOBOO_ID);
    if (!koobooId) return;
    let list: HTMLElement[] = [];

    for (const i of parent!.children as any) {
      let child = i as HTMLElement;
      if (child.id == HOVER_BORDER_SKIP) continue;
      if (el.tagName == child.tagName) {
        list.push(child);
      }
    }

    if (list.length > 1) {
      return {
        list,
        parent,
        koobooId
      };
    }
  }
}
