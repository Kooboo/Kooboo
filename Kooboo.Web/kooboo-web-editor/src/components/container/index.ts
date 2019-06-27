import { HOVER_BORDER_SKIP } from "../../common/constants";

export function createContainer() {
  let el = document.createElement("div");
  el.id = HOVER_BORDER_SKIP;
  el.style.fontFamily = `BlinkMacSystemFont,"Segoe UI",Roboto,Oxygen-Sans,Ubuntu,Cantarell,"Helvetica Neue",sans-serif`;
  el.style.fontSize = "14px";
  document.body.appendChild(el);
  return el;
}
