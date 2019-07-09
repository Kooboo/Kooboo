import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getRepeatComment, hasOperation } from "../utils";
import { reload } from "@/dom/utils";
import { editRepeat } from "@/kooboo/outsideInterfaces";

export function createEditRepeatItem() {
  const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_REPEAT, MenuActions.editRepeat);

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;

    if (!getRepeatComment(args.koobooComments)) visiable = false;
    if (hasOperation(context.operationManager)) setReadonly();
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getRepeatComment(args.koobooComments)!;
    await editRepeat(comment.nameorid!, comment.folderid!);
    reload();
  });

  return { el, update };
}
