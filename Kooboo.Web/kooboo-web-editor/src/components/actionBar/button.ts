import { createDiv } from "@/dom/element";

interface ActionButton extends HTMLDivElement {
  changeIcon(icon: any): void;
}

export function createButton(icon: any, title: string) {
  let el = createDiv();
  el.classList.add("kb_action_bar_btn");
  el.title = title;
  el.style.backgroundImage = `url(${icon})`;
  let btn = el as ActionButton;
  btn.changeIcon = e => (el.style.backgroundImage = `url(${e})`);
  return btn;
}
