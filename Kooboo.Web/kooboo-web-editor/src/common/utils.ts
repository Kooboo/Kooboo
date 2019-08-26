import { getAllElement, isLink, isInEditorContainer } from "@/dom/utils";
import context from "./context";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { HOVER_BORDER_SKIP } from "./constants";
import { TEXT } from "./lang";
import { hover } from "@/dom/events";

try {
  require("@webcomponents/shadydom");
} catch (error) {}

export function delay(time: number) {
  return new Promise(rs => {
    setTimeout(rs, time);
  });
}

export function setElement() {
  for (const i of getAllElement(document.body, true)) {
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

    if (context.editing || isInEditorContainer(e)) return;
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
  document.documentElement.appendChild(el);
  let shadow = el.attachShadow({ mode: "open" });
  let root = document.createElement("div");
  root.id = HOVER_BORDER_SKIP;
  root.style.wordBreak = "break-all";
  root.style.fontFamily = `"Times New Roman",Times,serif`;
  root.style.fontStyle = "normal";
  root.style.fontVariant = "normal";
  root.style.lineHeight = "16px";
  shadow.appendChild(root);
  return root;
}

export function browserCheck() {
  let isChrome = navigator.userAgent.toLowerCase().indexOf("chrome") > -1;
  if (!isChrome) alert(TEXT.USE_CHROME_TIP);
}
