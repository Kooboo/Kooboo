import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getRepeatComment, hasOperation } from "../utils";
import { reload } from "@/dom/utils";
import { editRepeat } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getRelatedRepeatComment } from "@/kooboo/utils";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditRepeatItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_REPEAT, MenuActions.editRepeat);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
    this.setReadonly = setReadonly;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  setReadonly: () => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!getRepeatComment(comments) && !getRelatedRepeatComment(args.element)) return this.setVisiable(false);
    if (hasOperation(context.operationManager)) return this.setReadonly();
  }

  async click() {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getRepeatComment(comments);
    if (!comment) comment = getRelatedRepeatComment(args.element);
    await editRepeat(comment!.nameorid!, comment!.folderid!);
    reload();
  }
}
