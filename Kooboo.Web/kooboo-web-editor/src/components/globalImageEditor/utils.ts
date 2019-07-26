import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";
import context from "@/common/context";

export function setImagePreview(preview: HTMLDivElement, el: HTMLElement) {
  preview.style.width = "150px";
  preview.style.height = "150px";
  preview.style.display = "inline-block";
  preview.style.margin = "15px";

  preview.onmouseover = () => {
    context.lastMouseEventArg = new MouseEvent("");
    el.scrollIntoView();
    emitHoverEvent(el);
    emitSelectedEvent();
  };
}
