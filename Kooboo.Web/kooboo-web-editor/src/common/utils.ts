import { getAllElement, isLink } from "@/dom/utils";
import context from "./context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { HOVER_BORDER_SKIP } from "./constants";
import { hover } from "@/dom/events";
import { TEXT } from "./lang";

export function delay(time: number) {
  return new Promise(rs => {
    setTimeout(rs, time);
  });
}

export function initElements() {
  for (const i of getAllElement(document.body, true)) {
    if (i.id == HOVER_BORDER_SKIP) continue;
    if (i instanceof HTMLElement) {
      if (isLink(i)) {
        let a = i.cloneNode(true);
        (a as any)._a = i;
        i.parentElement!.replaceChild(a, i);
      }
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
  el.style.cssText = "all:unset !important";
  el.style.fontSize = "16px";
  el.id = HOVER_BORDER_SKIP;
  document.documentElement.appendChild(el);
  let shadow = el.attachShadow({ mode: "open" });
  let root = document.createElement("div");
  root.style.wordBreak = "break-all";
  root.style.fontFamily = `"Times New Roman",Times,serif`;
  root.style.fontStyle = "normal";
  root.style.fontVariant = "normal";
  root.style.lineHeight = "16px";
  shadow.appendChild(root);
  return root;
}

export function htmlModeCheck() {
  if (!document.doctype) alert(TEXT.HTML_MODE_TIP);
  return !!document.doctype;
}
