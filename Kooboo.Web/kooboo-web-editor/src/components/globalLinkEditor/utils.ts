import { TEXT } from "@/common/lang";
import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";
import { createButton, createDiv } from "@/dom/element";
import context from "@/common/context";

export function createLinkItem(el: HTMLElement, onEdit: () => void) {
  let item = createDiv();
  item.classList.add("kb_web_editor_global_link_editor_item");
  item.onmouseover = () => {
    context.lastMouseEventArg = new MouseEvent("");
    el.scrollIntoView();
    emitHoverEvent(el);
    emitSelectedEvent();
  };
  let label = createDiv();
  label.innerText = el.getAttribute("href")!;
  let button = createButton(TEXT.EDIT);
  button.onclick = onEdit;
  item.appendChild(label);
  item.appendChild(button);
  return {
    item,
    setLabel: (s: string) => (label.innerText = s)
  };
}
