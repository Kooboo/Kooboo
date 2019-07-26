import { TEXT } from "../../common/lang";
import closeIcon from "../../assets/icons/guanbi.svg";
import context from "../../common/context";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import expandIcon from "@/assets/icons/fangda.svg";
import { STANDARD_Z_INDEX } from "@/common/constants";
import { createDiv, createImg } from "@/dom/element";
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
  el.addEventListener("click", () => context.floatMenuClickEvent.emit(type));

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

export function createContainer() {
  let el = createDiv();
  el.classList.add("kb_web_editor_menu");
  el.style.zIndex = STANDARD_Z_INDEX + 1 + "";
  let { setExpandBtnVisiable, title } = createTitle();
  el.append(title);

  const updatePosition = (x: number, y: number, pageHeight: number, pageWidth: number) => {
    let rect = el.getBoundingClientRect();

    if (x + rect.width > pageWidth) {
      x = x - rect.width + 3;
    }

    if (y + rect.height > pageHeight) {
      y = y - rect.height + 3;
    }

    el.style.top = y + "px";
    el.style.left = x + "px";
  };

  return {
    container: el,
    updatePosition,
    setExpandBtnVisiable
  };
}

function createTitle() {
  const el = createDiv();
  el.classList.add("kb_web_editor_menu_title");
  el.appendChild(document.createTextNode(TEXT.MENU));
  el.appendChild(createCloseButton());
  let expandButton = createExpandButton();
  el.appendChild(expandButton);
  const setExpandBtnVisiable = (visiable: boolean) => {
    expandButton.style.display = visiable ? "block" : "none";
  };
  return { title: el, setExpandBtnVisiable };
}

function createCloseButton() {
  const el = createImg();
  el.src = closeIcon;
  el.style.height = "16px";
  el.style.cssFloat = "right";
  el.onclick = () => context.floatMenuClickEvent.emit(MenuActions.close);
  return el;
}

function createExpandButton() {
  const el = createImg();
  el.src = expandIcon;
  el.title = TEXT.EXPAND_SELECTION;
  el.style.height = "16px";
  el.style.cssFloat = "right";
  el.style.marginRight = "8px";
  el.onclick = () => context.floatMenuClickEvent.emit(MenuActions.expand);
  return el;
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
