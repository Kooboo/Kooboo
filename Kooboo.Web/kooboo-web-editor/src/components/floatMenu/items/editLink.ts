import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isDynamicContent, getCleanParent, isDirty } from "@/kooboo/utils";
import { isLink } from "@/dom/utils";
import { getViewComment, getUrlComment, updateDomLink, updateUrlLink, updateAttributeLink, getAttributeComment, getRepeatComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditLinkItem extends BaseMenuItem {
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
    if (getAttributeComment(comments)) return this.setVisiable(false);
    if (getRepeatComment(comments)) return this.setVisiable(false);
    if (getUrlComment(comments)) return this.setVisiable(true);
    if (!isLink(args.element)) return this.setVisiable(false);
    if (!getViewComment(comments)) return this.setVisiable(false);
    if (isDynamicContent(args.element)) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let urlComment = getUrlComment(comments);
    let viewComment = getViewComment(comments)!;
    let { koobooId, parent } = getCleanParent(args.element);
    if (isDirty(args.element) && parent) {
      updateDomLink(parent, koobooId!, args.element, viewComment);
    } else if (urlComment) {
      updateUrlLink(args.element, args.koobooId!, urlComment, viewComment);
    } else {
      updateAttributeLink(args.element, args.koobooId!, viewComment);
    }
  }
}
