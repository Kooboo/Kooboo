import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { updateDomImage, getEditableComment } from "../utils";
import { getUnpollutedEl, isDynamicContent } from "@/kooboo/utils";
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
    let { element } = context.lastSelectedDomEventArgs;
    if (!isImg(element)) return this.setVisiable(false);
    let aroundComments = KoobooComment.getAroundComments(element);
    if (aroundComments.find(f => f.getValue("attribute") == "src")) return this.setVisiable(false);
    if (!getEditableComment(comments)) return this.setVisiable(false);
    let el = getUnpollutedEl(element);
    if (!el || isDynamicContent(el)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    updateDomImage(element as HTMLImageElement);
  }
}
