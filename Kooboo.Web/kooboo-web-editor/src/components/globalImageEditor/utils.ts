import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";

export function setImagePreview(preview: HTMLDivElement, el: HTMLElement) {
  preview.style.width = "150px";
  preview.style.height = "150px";
  preview.style.display = "inline-block";
  preview.style.margin = "15px";

  preview.onmouseover = () => {
    el.scrollIntoView();
    emitHoverEvent(el);
    emitSelectedEvent();
  };
}
