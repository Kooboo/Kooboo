import { STANDARD_Z_INDEX } from "@/common/constants";
import { createDiv } from "@/dom/element";

export function createContainer(width?: string) {
  let el = createDiv();
  el.classList.add("kb_web_editor_modal");
  el.style.zIndex = STANDARD_Z_INDEX + 5 + "";
  let win = createDiv();
  win.classList.add("kb_web_editor_modal_window");
  if (width != undefined) win.style.width = width;
  el.appendChild(win);
  return {
    shade: el,
    win
  };
}
