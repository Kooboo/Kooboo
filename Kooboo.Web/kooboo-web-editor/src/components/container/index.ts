import { HOVER_BORDER_SKIP } from "@/common/constants";
import { createDiv } from "@/dom/element";

export function createContainer() {
  let el = createDiv();
  el.id = HOVER_BORDER_SKIP;
  document.documentElement.appendChild(el);
  return el;
}
