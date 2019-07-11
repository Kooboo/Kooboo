import { FONT_FAMILY } from "./utils";

export default function createSpan() {
  let el = document.createElement("span");
  el.style.display = "inline-block";
  el.style.fontSize = "12px";
  el.style.fontFamily = FONT_FAMILY;
  el.style.color = "#5f5f5f";
  el.style.border = "none";
  el.style.outline = "none";
  el.style.position = "static";
  return el;
}
