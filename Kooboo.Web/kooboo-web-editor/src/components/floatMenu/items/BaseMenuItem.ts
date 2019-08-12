import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "../menu";
import { createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";

export default abstract class BaseMenuItem implements MenuItem {
  constructor(protected parentMenu: Menu) {}

  abstract el: HTMLElement;

  abstract setVisiable: (visiable: boolean) => void;

  abstract update(comments: KoobooComment[]): void;

  createItem(text: string) {
    let el = createDiv();
    el.classList.add("kb_web_editor_menu_item");
    el.append(text);
    let warn: HTMLDivElement;

    const setVisiable = (visiable: boolean) => {
      el.style.display = visiable ? "block" : "none";
    };

    const setReadonly = (readonly?: boolean) => {
      if (!warn) warn = this.createWarn();
      if (readonly) {
        el.style.color = "#ccc";
        el.style.pointerEvents = "none";
        el.appendChild(warn);
      } else {
        el.style.removeProperty("color");
        el.style.removeProperty("pointer-events");
        if (el.contains(warn)) el.removeChild(warn);
      }
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
