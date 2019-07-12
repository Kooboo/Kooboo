import { FONT_FAMILY } from "./utils";

export default function createDiv() {
  let el = document.createElement("div");
  el.style.fontFamily = FONT_FAMILY;
  el.style.fontSize = "14px";
  el.style.color = "#5f5f5f";
  el.style.border = "none";
  el.style.outline = "none";
  el.style.display = "block";
  el.style.position = "static";
  el.style.backgroundColor = "rgba(255,255,255,0)";
  el.style.fontWeight = "400";
  return el;
}
