import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getRepeat, hasOperation } from "../utils";
import { editRepeat } from "@/common/outsideInterfaces";
import { reload } from "@/dom/utils";

export function createEditRepeatItem() {
  const { el, setVisiable } = createItem(
    TEXT.EDIT_REPEAT,
    MenuActions.editRepeat
  );

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;

    if (!getRepeat(args.koobooComments)) visiable = false;
    if (hasOperation(context.operationManager)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getRepeat(args.koobooComments)!;
    await editRepeat(comment.nameorid!, comment.folderid!);
    reload();
  });

  return { el, update };
}
