import { getParentElements } from "@/dom/utils";
import { getCleanParent, isDynamicContent } from "@/kooboo/utils";
import { isEditable } from "../floatMenu/utils";
import { HOVER_BORDER_SKIP } from "@/common/constants";

export type editableData = { list: HTMLElement[]; cleanParent: HTMLElement; koobooId: string };
export function getEditableData(element: HTMLElement): editableData | undefined {
  let els = getParentElements(element, true);
  if (els.length > 5) els = els.splice(0, 5);
  for (const el of els) {
    let { parent: cleanParent, koobooId } = getCleanParent(el);
    let parent = el.parentElement;
    if (!cleanParent || !koobooId || !parent || !isEditable(el) || isDynamicContent(parent)) return;
    let list: HTMLElement[] = [];
    for (const i of el.parentElement!.children as any) {
      let child = i as HTMLElement;
      if (child.id == HOVER_BORDER_SKIP) continue;
      if (el.tagName == child.tagName) {
        list.push(child);
      }
    }
    if (list.length > 1) {
      return {
        list,
        cleanParent,
        koobooId
      };
    }
  }
}
