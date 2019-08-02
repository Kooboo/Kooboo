import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isLink } from "@/dom/utils";
import { getAttributeComment, updateAttributeLink } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditRepeatLinkItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!isLink(args.element)) return this.setVisiable(false);
    let comment = getAttributeComment(comments, "href");
    if (!comment || !comment.fieldname) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getAttributeComment(comments, "href");
    updateAttributeLink(args.element, args.koobooId!, comment!);
  }
}
