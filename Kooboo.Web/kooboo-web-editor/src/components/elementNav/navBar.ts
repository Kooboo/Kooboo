import { createDiv } from "@/dom/element";
import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";
import context from "@/common/context";
import { STANDARD_Z_INDEX } from "@/common/constants";

export function createNavBar() {
  const el = createDiv();
  el.classList.add("kb_web_editor_element_nav_bar");
  el.style.zIndex = STANDARD_Z_INDEX + "";
  el.draggable = true;
  let temp: { x: number; y: number };
  el.ondragstart = (e: DragEvent) => {
    let rect = el.getBoundingClientRect();
    temp = { x: rect.left + rect.width - e.x, y: e.y - rect.top };
  };
  el.ondrag = e => {
    if (e.x == 0 || e.y == 0) return;
    el.style.right = document.documentElement.clientWidth - e.x - temp.x + "px";
    el.style.top = e.y - temp.y + "px";
  };

  const update = (elements: HTMLElement[]) => {
    el.innerHTML = "";
    el.style.display = "inline-block";
    for (const i of elements) {
      let item = createDiv();
      if (i.tagName != "HTML") {
        item.onclick = e => {
          context.lastMouseEventArg = e;
          emitSelectedEvent();
          e.stopPropagation();
        };

        item.onmouseover = e => {
          emitHoverEvent(i);
          e.stopPropagation();
        };
      }
      item.innerText = i.tagName;
      el.appendChild(item);
      if (i != elements[elements.length - 1]) el.appendChild(document.createTextNode("<"));
    }
  };
  return { el, update };
}
