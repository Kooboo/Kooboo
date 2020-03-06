import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { getWrapDom } from "@/kooboo/utils";
import { editMenu } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditMenuItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable, setReadonly } = this.createItem(TEXT.EDIT_MENU);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    if (!comments.find(f => f.source == "menu")) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(element);
    let comment = comments.find(f => f.source == "menu")!;
    var { startNode, endNode } = getWrapDom(element, "menu");
    if (!startNode || !endNode) return;
    editMenu(comment.getValue("id")!, c => {
      let temp = createDiv();

      // eslint-disable-next-line no-constant-condition
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
