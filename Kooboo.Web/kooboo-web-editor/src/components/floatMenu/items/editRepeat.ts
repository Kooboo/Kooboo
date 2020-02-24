import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { hasOperation, getRepeatSourceComment } from "../utils";
import { reload } from "@/dom/utils";
import { editRepeat } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { getWrapDom } from "@/kooboo/utils";

export default class EditRepeatItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable, setReadonly } = this.createItem(TEXT.EDIT_REPEAT);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
    this.setReadonly = setReadonly;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  setReadonly: (readonly: boolean) => void;

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let { nodes } = getWrapDom(element, "repeatitem");
    let comments = KoobooComment.getInnerComments(nodes);
    if (!getRepeatSourceComment(comments, "textcontent")) return this.setVisiable(false);
    this.setReadonly(hasOperation(context.operationManager));
  }

  async click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let { nodes } = getWrapDom(element, "repeatitem");
    let comments = KoobooComment.getInnerComments(nodes);
    let comment = getRepeatSourceComment(comments, "textcontent")!;
    await editRepeat(comment.id, "");
    reload();
  }
}
