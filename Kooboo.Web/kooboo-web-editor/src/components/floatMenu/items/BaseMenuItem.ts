import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "../menu";
import { createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";

export default abstract class BaseMenuItem implements MenuItem {
  constructor(public parentMenu: Menu) {}

  abstract el: HTMLElement;

  abstract setVisiable: (visiable: boolean) => void;

  abstract update(comments: KoobooComment[]): void;

  createItem(text: string) {
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
      el.appendChild(this.createWarn());
    };
  
    return {
      el,
      setVisiable,
      setReadonly
    };
  }
  
  createWarn() {
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
}

export interface MenuItem {
  el: HTMLElement;
  update(comments: KoobooComment[]): void;
}