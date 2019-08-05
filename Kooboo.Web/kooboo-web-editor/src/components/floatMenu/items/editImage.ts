import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment, getViewComment, updateAttributeImage, updateDomImage, getAttributeComment } from "../utils";
import { isDynamicContent, getCleanParent, isDirty } from "@/kooboo/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditImageItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT_IMAGE);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) return this.setVisiable(false);
    if (getAttributeComment(comments)) return this.setVisiable(false);
    if (!getViewComment(comments)) return this.setVisiable(false);
    if (isDynamicContent(args.element)) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    
    let comments = KoobooComment.getComments(args.element);
    let el = args.element as HTMLImageElement;
    let { koobooId, parent } = getCleanParent(args.element);
    if (isDirty(args.element) && parent) {
      updateDomImage(el, parent, koobooId!, getViewComment(comments)!);
    } else {
      updateAttributeImage(el, args.koobooId!, getEditComment(comments)!);
    }
  }
}
