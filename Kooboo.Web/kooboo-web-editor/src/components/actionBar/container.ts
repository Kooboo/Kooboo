import { createDiv } from "@/dom/element";
import { STANDARD_Z_INDEX } from "@/common/constants";
import context from "@/common/context";

export function createContainer() {
  let container = createDiv();
  container.style.position = "fixed";
  container.style.width = "60px";
  container.style.top = "60px";
  container.style.right = "60px";
  container.style.zIndex = STANDARD_Z_INDEX + "";
  context.editableEvent.addEventListener(e => {
    container.style.display = e ? "none" : "block";
  });

  return container;
}
