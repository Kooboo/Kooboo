import { STANDARD_Z_INDEX } from "@/common/constants";

export function createContainer() {
  let el = document.createElement("div");
  let win = createWindow();
  el.appendChild(win);
  applyStyle(el.style);
  el.ontouchmove = e => e.stopPropagation();
  return { el, win };
}

function applyStyle(style: CSSStyleDeclaration) {
  style.backgroundColor = "rgba(255,255,255,.75)";
  style.position = "fixed";
  style.top = "0px";
  style.bottom = "0px";
  style.left = "0px";
  style.right = "0px";
  style.zIndex = STANDARD_Z_INDEX + 4 + "";
}

export function createWindow() {
  let el = document.createElement("div");
  applyWindowStyle(el.style);
  return el;
}

function applyWindowStyle(style: CSSStyleDeclaration) {
  style.backgroundColor = "#fff";
  style.borderColor = "1px solid #ccc";
  style.borderRadius = "3px";
  style.boxShadow = "0 0 10px 1px rgba(34,47,62,.15)";
  style.width = "60%";
  style.maxHeight = "60%";
  style.position = "absolute";
  style.margin = "auto";
  style.top = "0px";
  style.bottom = "0px";
  style.left = "0px";
  style.right = "0px";
  style.padding = "16px";
  style.overflowY = "hidden";
}
