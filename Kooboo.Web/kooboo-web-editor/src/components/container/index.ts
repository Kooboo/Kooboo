import { HOVER_BORDER_SKIP } from "@/common/constants";

export function createContainer() {
  let el = document.createElement("div");
  el.id = HOVER_BORDER_SKIP;
  el.style.fontFamily = `"Segoe UI,BlinkMacSystemFont,Roboto,Oxygen-Sans,Ubuntu,Cantarell,"Helvetica Neue",sans-serif"`;
  el.style.fontSize = "14px";
  el.style.color = "#5f5f5f";
  el.style.backgroundColor = "#FFF";
  document.documentElement.appendChild(el);
  return el;
}
