import { TEXT } from "../../common/lang";
import closeIcon from "../../assets/icons/guanbi.svg";
import context from "../../common/context";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import expandIcon from "@/assets/icons/fangda.svg";
import { STANDARD_Z_INDEX } from "@/common/constants";
import createDiv from "@/dom/div";

export interface MenuItem {
  el: HTMLElement;
  update(): void;
}

export function createItem(text: string, type: MenuActions) {
  let el = createDiv();
  let isReadonly = false;
  el.style.padding = "5px 10px";
  el.style.borderBottom = "1px solid #eee";
  el.style.cursor = "default";
  el.style.fontSize = "12px";
  el.append(text);
  el.addEventListener("click", () => context.floatMenuClickEvent.emit(type));

  const setVisiable = (visiable: boolean) => {
    el.style.display = visiable ? "block" : "none";
  };

  const setReadonly = () => {
    if (isReadonly) return;
    isReadonly = true;
    el.style.color = "#ccc";
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
  el.style.position = "absolute";
  el.style.width = "150px";
  el.style.minHeight = "50px";
  el.style.borderRadius = "3px";
  el.style.fontSize = "12px";
  el.style.opacity = "0.9";
  el.style.overflow = "hidden";
  el.style.boxShadow = "0 0 3px #ddd";
  el.style.backgroundColor = "#fff";
  el.style.display = "none";
  el.style.zIndex = STANDARD_Z_INDEX + 1 + "";
  el.append(createTitle());

  const updatePosition = (x: number, y: number, pageHeight: number, pageWidth: number) => {
    let rect = el.getBoundingClientRect();

    if (x + rect.width > pageWidth) {
      x -= rect.width;
    }

    if (y + rect.height > pageHeight) {
      y -= rect.height;
    }
    el.style.top = y + "px";
    el.style.left = x + "px";
  };

  return {
    el,
    updatePosition
  };
}

function createTitle() {
  const el = createDiv();
  el.style.padding = "5px 10px";
  el.style.color = "#fff";
  el.style.backgroundColor = "rgb(2, 154, 214)";
  const text = createDiv();
  text.style.color = "#fff";
  text.style.display = "inline-block";
  text.style.fontSize = "12px";
  text.innerText = TEXT.MENU;
  el.appendChild(text);
  el.appendChild(createCloseButton());
  el.appendChild(createExpandButton());
  return el;
}

function createCloseButton() {
  const el = document.createElement("img");
  el.src = closeIcon;
  el.style.height = "16px";
  el.style.cssFloat = "right";
  el.onclick = () => context.floatMenuClickEvent.emit(MenuActions.close);
  return el;
}

function createExpandButton() {
  const el = document.createElement("img");
  el.src = expandIcon;
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
  el.title = TEXT.PLEASE_EDIT_AFTER_SAVE;
  el.onclick = e => {
    e.stopPropagation();
  };
  return el;
}
