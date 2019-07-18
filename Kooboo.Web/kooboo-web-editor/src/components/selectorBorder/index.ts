import context from "@/common/context";
import { createRect } from "./rect";
import { HOVER_BORDER_WIDTH, HOVER_BORDER_COLOR, SELECTED_BORDER_COLOR, SELECTED_BORDER_WIDTH } from "@/common/constants";

export function createHoverBorder() {
  const rect = createRect(HOVER_BORDER_WIDTH, HOVER_BORDER_COLOR);
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

export function createSelectedBorder() {
  const rect = createRect(SELECTED_BORDER_WIDTH, SELECTED_BORDER_COLOR);
  context.domChangeEvent.addEventListener(e => {
    if (!e.element || context.editing) {
      rect.hidden();
      return;
    }
    rect.updatePosition(e.element);
  });

  context.editableEvent.addEventListener(e => {
    if (e) rect.hidden();
  });

  return rect.el;
}
