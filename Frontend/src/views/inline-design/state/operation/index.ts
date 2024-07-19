import { getElements } from "@/utils/dom";
import { K_ATTRIBUTE_PLACEHOLDER } from "../../constants";
import { doc } from "@/views/inline-design/page";
import type { Change } from "../change";
export { createDomOperation } from "./dom-operation";

export function getElementById(id: string) {
  if (!doc.value) return;
  const elements = getElements(doc.value);

  for (const el of elements) {
    const elementId = el.getAttribute(K_ATTRIBUTE_PLACEHOLDER);
    if (!elementId) continue;
    if (elementId.includes(id)) return el;
  }
}

export interface Operation<T extends Change> {
  changes: T[];
  undo(): void;
  redo(): void;
}
