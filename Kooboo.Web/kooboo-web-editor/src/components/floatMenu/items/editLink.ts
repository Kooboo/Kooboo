import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isLink } from "@/dom/utils";
import { updateDomLink, getEditableComment, ElementAnalyze } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { getUnpollutedEl, isDynamicContent } from "@/kooboo/utils";

export default class EditLinkItem extends BaseMenuItem {
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
    let { operability, kooobooIdEl, fieldComment } = ElementAnalyze(element);

    let aroundComments = KoobooComment.getAroundComments(element);
    if (aroundComments.find(f => f.getValue("attribute") == "href")) return this.setVisiable(false);
    if (!isLink(element) || !operability) return this.setVisiable(false);
    if (!kooobooIdEl && !fieldComment) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    updateDomLink(element);
  }
}
