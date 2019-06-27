import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { hasOperation, getMenu } from "../utils";
import { editRepeat, editMenu } from "@/common/outsideInterfaces";
import { reload } from "@/dom/utils";

export function createEditMenuItem() {
  const { el, setVisiable } = createItem(TEXT.EDIT_MENU, MenuActions.editMenu);

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;

    if (!getMenu(args.koobooComments)) visiable = false;
    if (hasOperation(context.operationManager)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getMenu(args.koobooComments)!;
    editMenu(comment.nameorid!, c => {
      let aa = document.createElement("div");
      aa.innerHTML = c;
      args.element.innerHTML = aa.children.item(0)!.innerHTML;
    });

    // reload();
  });

  return { el, update };
}
