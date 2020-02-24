import { TEXT } from "@/common/lang";
import context from "@/common/context";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class ClickItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.CLICK);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(): void {
    this.setVisiable(true);
  }

  click() {
    let element = context.lastHoverDomEventArgs.element;
    this.parentMenu.hidden();

    if ((element as any)._a) (element as any)._a.click();
    else element.click();
  }
}
