import context from "@/common/context";
import { createShade } from "./shade";

export function createEditorShade() {
  let { el, hidden, updatePosition } = createShade();

  context.editableEvent.addEventListener(display => {
    if (!context.lastSelectedDomEventArgs) return;

    if (display) {
      updatePosition(context.lastSelectedDomEventArgs.element);
    } else {
      hidden();
    }
  });

  context.tinymceInputEvent.addEventListener(() => {
    if (!context.lastSelectedDomEventArgs) return;
    updatePosition(context.lastSelectedDomEventArgs.element);
  });

  document.addEventListener("scroll", () => {
    if (context.editing) {
      updatePosition(context.lastSelectedDomEventArgs.element);
    }
  });

  return el;
}
