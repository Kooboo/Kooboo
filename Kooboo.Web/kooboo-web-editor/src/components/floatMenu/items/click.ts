import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import BaseMenuItem from "./BaseMenuItem";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "../menu";

export default class ClickItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = createItem(TEXT.CLICK, MenuActions.click);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
  }

  click() {
    let element = context.lastHoverDomEventArgs.element;
    this.parentMenu.hidden();

    if ((element as any)._a) (element as any)._a.click();
    else element.click();
  }
}
