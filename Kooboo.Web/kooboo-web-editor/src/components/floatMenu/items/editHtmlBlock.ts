import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { getHtmlBlockComment, hasOperation } from "../utils";
import { editHtmlBlock } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

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

  setReadonly: () => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    if (!getHtmlBlockComment(comments)) return this.setVisiable(false);
    if (hasOperation(context.operationManager)) {
      this.setReadonly();
    }
  }

  async click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(args.element);
    let comment = getHtmlBlockComment(comments)!;
    let nameorid = comment.nameorid;
    await editHtmlBlock(nameorid!);
  }
}
