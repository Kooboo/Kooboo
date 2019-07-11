import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getMenuComment } from "../utils";
import { getWrapDom } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { editMenu } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import createDiv from "@/dom/div";

export function createEditMenuItem() {
  const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_MENU, MenuActions.editMenu);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (!getMenuComment(comments)) setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
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
  });

  return { el, update };
}
