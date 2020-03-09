import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { hasOperation } from "../utils";
import { editHtmlBlock } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { reload } from "@/dom/utils";

export default class EditHtmlBlockItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable, setReadonly } = this.createItem(TEXT.EDIT_HTML_BLOCK);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
    this.setReadonly = setReadonly;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  setReadonly: (readonly: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    if (!comments.find(f => f.source == "htmlblock")) return this.setVisiable(false);
    this.setReadonly(hasOperation(context.operationManager));
  }

  async click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(element);
    let comment = comments.find(f => f.source == "htmlblock")!;
    await editHtmlBlock(comment.getValue("id")!);
    reload();
  }
}
