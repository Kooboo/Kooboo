import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getMenuComment } from "../utils";
import { getWrapDom } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { editMenu } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditMenuItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_MENU, MenuActions.editMenu);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    if (!getMenuComment(comments)) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    
    let comments = KoobooComment.getComments(args.element);
    let comment = getMenuComment(comments)!;
    var { startNode, endNode } = getWrapDom(args.element, OBJECT_TYPE.menu);
    if (!startNode || !endNode) return;
    editMenu(comment.nameorid!, c => {
      let temp = createDiv();

      while (true) {
        if (startNode!.nextSibling == endNode) {
          startNode!.parentElement!.insertBefore(temp, endNode!);
          break;
        }
        startNode!.parentElement!.removeChild(startNode!.nextSibling!);
      }

      temp.outerHTML = c;
    });
  }
}
