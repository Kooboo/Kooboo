import { getAllElement } from "@/dom/utils";
import context from "./context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { HOVER_BORDER_SKIP } from "./constants";
import { hover } from "@/dom/events";

export function delay(time: number) {
  return new Promise(rs => {
    setTimeout(rs, time);
  });
}

export function initElements() {
  for (const i of getAllElement(document.body, true)) {
    if (i.id == HOVER_BORDER_SKIP) continue;
    if (i instanceof HTMLElement) {
      // if (isLink(i)) {
      //   let a = i.cloneNode(true);
      //   (a as any)._a = i;
      //   i.parentElement!.replaceChild(a, i);
      // }
      i.addEventListener("mouseover", hover);
      holdUpClick(i);
    }
  }
}

export function holdUpClick(el: HTMLElement) {
  el.onclick = e => {
    if (e.isTrusted) {
      e.stopPropagation();
      e.preventDefault();
    }
    if (context.editing) return;
    let element = context.lastHoverDomEventArgs.closeElement;
    var args = new SelectedDomEventArgs(element);
    context.lastMouseEventArg = e;
    context.domChangeEvent.emit(args);
  };
}

export function createContainer() {
  let el = document.createElement("div");
  el.classList.add("kb_web_editor_container");
  el.id = HOVER_BORDER_SKIP;
  document.documentElement.appendChild(el);
  let shadow = el.attachShadow({ mode: "open" });
  let root = document.createElement("div");
  root.classList.add("kb_web_editor_root");
  shadow.appendChild(root);
  return root;
}
