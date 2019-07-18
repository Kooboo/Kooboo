import { createDiv, createImg } from "@/dom/element";

interface ActionButton extends HTMLDivElement {
  changeIcon(icon: any): void;
}

export function createButton(icon: any, title: string) {
  let el = createDiv();
  el.classList.add("kb_action_bar_btn");
  el.title = title;
  let img = createImg();
  img.src = icon;
  el.appendChild(img);
  let btn = el as ActionButton;
  btn.changeIcon = e => (img.src = e);
  return btn;
}
