import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isLink } from "@/dom/utils";
import { updateAttributeLink } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditRepeatLinkItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT_LINK);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let aroundComments = KoobooComment.getAroundComments(element);
    if (!isLink(element)) return this.setVisiable(false);
    if (!aroundComments.find(f => f.attribute == "href" && f.source != "none")) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    updateAttributeLink(element);
  }
}
