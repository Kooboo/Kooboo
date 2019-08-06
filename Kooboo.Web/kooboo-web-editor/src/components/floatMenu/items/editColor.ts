import { TEXT } from "@/common/lang";
import context from "@/common/context";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
// import { createColorEditor } from "@/components/colorEditor";

export default class EditColorItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable, setReadonly } = this.createItem(TEXT.EDIT_COLOR);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
    this.setReadonly = setReadonly;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  setReadonly: () => void;

  update(): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!args.koobooId) return this.setVisiable(false);
  }

  async click() {
    let args = context.lastHoverDomEventArgs;
    //createColorEditor(args.element);
    this.parentMenu.hidden();
  }
}
