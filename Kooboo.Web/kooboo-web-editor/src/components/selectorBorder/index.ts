import context from "../../common/context";
import { createRect } from "./rect";

export function createHoverBorder() {
  const rect = createRect();
  context.hoverDomEvent.addEventListener(e => {
    if (!e.closeElement || context.editing) {
      rect.hidden();
      return;
    }
    rect.updatePosition(e.closeElement);
  });

  context.editableEvent.addEventListener(e => {
    if (e) rect.hidden();
  });

  return rect.el;
}
