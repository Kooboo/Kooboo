import { STANDARD_Z_INDEX } from "@/common/constants";
import { createDiv } from "@/dom/element";

export function createContainer(document: Document) {
  let container = createDiv();
  container.style.position = "fixed";
  container.style.width = "60px";
  container.style.top = "60px";
  container.style.left = document.body.scrollWidth - 120 + "px";
  container.style.zIndex = STANDARD_Z_INDEX - 1 + "";
  return container;
}
