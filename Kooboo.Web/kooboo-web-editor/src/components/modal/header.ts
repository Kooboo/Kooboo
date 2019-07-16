import { createDiv } from "@/dom/element";

export function createHeader(title: string, parent: HTMLElement) {
  var el = createDiv();
  el.innerText = title;
  el.style.cursor = "move";
  el.draggable = true;
  let temp: { x: number; y: number };
  el.ondragstart = (e: DragEvent) => {
    let rect = parent.getBoundingClientRect();
    temp = { x: e.x - rect.left, y: e.y - rect.top };
  };

  el.ondrag = e => {
    if (e.x == 0 || e.y == 0) return;
    parent.style.marginLeft = e.x - temp.x + "px";
    parent.style.marginTop = e.y - temp.y + "px";
  };
  applyStyle(el.style);
  return el;
}

function applyStyle(style: CSSStyleDeclaration) {
  style.fontSize = "20px";
  style.fontWeight = "400";
  style.lineHeight = "1.3";
  style.fontStyle = "normal";
  style.padding = "20px 20px";
}
