import { createDiv } from "@/dom/element";

interface ActionButton extends HTMLDivElement {
  changeIcon(icon: any): void;
}

export function createButton(document: Document, icon: any, title: string) {
  let el = createDiv();
  el.classList.add("kb_action_bar_btn");
  el.title = title;
  let img = document.createElement("img");
  img.style.width = "50%";
  img.style.marginLeft = "25%";
  img.style.height = "50%";
  img.style.marginTop = "25%";
  img.src = icon;
  el.appendChild(img);
  let btn = el as ActionButton;
  btn.changeIcon = e => (img.src = e);
  return btn;
}
