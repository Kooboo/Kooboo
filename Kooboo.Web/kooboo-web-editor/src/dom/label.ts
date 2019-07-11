import { FONT_FAMILY } from "./utils";

export function createLabel(text: string) {
  let el = document.createElement("label");
  el.style.lineHeight = "24px";
  el.style.padding = "5px 4.75px";
  el.style.margin = "0";
  el.style.fontFamily = FONT_FAMILY;
  el.style.border = "none";
  el.style.outline = "none";
  el.style.fontSize = "14px";
  el.style.color = "#5f5f5f";
  el.style.display = "inline-block";
  el.innerText = text;
  return el;
}
