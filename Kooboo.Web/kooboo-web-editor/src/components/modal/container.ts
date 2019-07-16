import { STANDARD_Z_INDEX } from "@/common/constants";
import { createDiv } from "@/dom/element";

export function createContainer(width?: string) {
  let el = createDiv();
  let win = createWindow();
  if (width != undefined) win.style.width = width;
  el.appendChild(win);
  applyStyle(el.style);
  return {
    shade: el,
    win
  };
}

function applyStyle(style: CSSStyleDeclaration) {
  style.backgroundColor = "rgba(255,255,255,.3)";
  style.position = "fixed";
  style.top = "0px";
  style.bottom = "0px";
  style.left = "0px";
  style.right = "0px";
  style.overflowY = "auto";
  style.zIndex = STANDARD_Z_INDEX + 4 + "";
}

export function createWindow() {
  let el = createDiv();
  applyWindowStyle(el.style);
  return el;
}

function applyWindowStyle(style: CSSStyleDeclaration) {
  style.backgroundColor = "#fff";
  style.borderColor = "1px solid #ccc";
  style.borderRadius = "3px";
  style.boxShadow = "0 0 10px 1px rgba(34,47,62,.15)";
  style.width = "60%";
  style.position = "relative";
  style.margin = "30px auto";
}
