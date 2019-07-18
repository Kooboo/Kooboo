import context from "@/common/context";
import { createNavBar } from "./navBar";
import { getParentElements } from "@/dom/utils";

export function createElementNav() {
  const { el, update } = createNavBar();
  context.domChangeEvent.addEventListener(e => {
    let elements = getParentElements(e.element);
    update(elements);
  });

  return el;
}
