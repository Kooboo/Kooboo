import { HOVER_BORDER_SKIP } from "../../common/constants";

export function createContainer() {
  let el = document.createElement("div");
  el.id = HOVER_BORDER_SKIP;
  document.body.appendChild(el);
  return el;
}
