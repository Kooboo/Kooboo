import { TEXT } from "../../common/lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { createDiv } from "@/dom/element";
import { KoobooComment } from "@/kooboo/KoobooComment";

export interface MenuItem {
  el: HTMLElement;
  update(comments: KoobooComment[]): void;
}

export function createItem(text: string, type: MenuActions) {
  let el = createDiv();
  el.classList.add("kb_web_editor_menu_item");
  let isReadonly = false;
  el.append(text);

  const setVisiable = (visiable: boolean) => {
    el.style.display = visiable ? "block" : "none";
  };

  const setReadonly = () => {
    if (isReadonly) return;
    isReadonly = true;
    el.style.color = "#ccc";
    el.style.pointerEvents = "none";
    el.appendChild(createWarn());
  };

  return {
    el,
    setVisiable,
    setReadonly
  };
}

function createWarn() {
  let el = createDiv();
  el.innerText = "!";
  el.style.display = "inline-block";
  el.style.width = "18px";
  el.style.height = "18px";
  el.style.color = "#fff";
  el.style.backgroundColor = "rgba(252, 56, 56, 0.74)";
  el.style.textAlign = "center";
  el.style.fontSize = "15px";
  el.style.fontWeight = "600";
  el.style.lineHeight = "18px";
  el.style.borderRadius = "9px";
  el.style.cssFloat = "right";
  el.style.pointerEvents = "painted";
  el.title = TEXT.PLEASE_EDIT_AFTER_SAVE;
  el.onclick = e => {
    e.stopPropagation();
  };
  return el;
}
