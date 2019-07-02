import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { hasOperation, getMenu } from "../utils";
import { reload } from "@/dom/utils";
import { getWrapDom } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { editMenu } from "@/kooboo/outsideInterfaces";

export function createEditMenuItem() {
  const { el, setVisiable, setReadonly } = createItem(
    TEXT.EDIT_MENU,
    MenuActions.editMenu
  );

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;
    if (!getMenu(args.koobooComments)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getMenu(args.koobooComments)!;
    var { startNode, endNode } = getWrapDom(args.element, OBJECT_TYPE.menu);
    if (!startNode || !endNode) return;
    editMenu(comment.nameorid!, c => {
      let temp = document.createElement("div");

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
