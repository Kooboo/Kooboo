import { TEXT } from "../../common/lang";
import closeIcon from "../../assets/icons/guanbi.svg";
import context from "../../common/context";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import expandIcon from "@/assets/icons/fangda.svg";
import { STANDARD_Z_INDEX } from "@/common/constants";

export interface MenuItem {
  el: HTMLElement;
  update(): void;
}

export function createItem(text: string, type: MenuActions) {
  let el = document.createElement("div");
  el.style.padding = "5px 10px";
  el.style.color = "#000";
  el.style.borderBottom = "1px solid #eee";
  el.style.cursor = "default";
  el.innerHTML = text;
  el.addEventListener("click", () => context.floatMenuClickEvent.emit(type));

  const setVisiable = (visiable: boolean) => {
    el.style.display = visiable ? "block" : "none";
  };

  return {
    el,
    setVisiable
  };
}

export function createContainer() {
  let el = document.createElement("div");
  el.style.position = "absolute";
  el.style.width = "150px";
  el.style.minHeight = "50px";
  el.style.borderRadius = "3px";
  el.style.fontSize = "12px";
  el.style.opacity = "0.9";
  el.style.overflow = "hidden";
  el.style.boxShadow = "0 0 3px #ddd";
  el.style.backgroundColor = "#fff";
  el.style.zIndex = STANDARD_Z_INDEX + 1 + "";
  el.append(createTitle());

  const updatePosition = (x: number, y: number) => {
    el.style.top = y + "px";
    el.style.left = x + "px";
  };

  return {
    el,
    updatePosition
  };
}

function createTitle() {
  const el = document.createElement("div");
  el.style.padding = "5px 10px";
  el.style.color = "#fff";
  el.style.backgroundColor = "rgb(2, 154, 214)";
  const text = document.createElement("span");
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