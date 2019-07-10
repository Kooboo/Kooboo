import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getRepeatComment, hasOperation } from "../utils";
import { reload } from "@/dom/utils";
import { editRepeat } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createEditRepeatItem() {
  const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_REPEAT, MenuActions.editRepeat);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (!getRepeatComment(comments)) setVisiable(false);
    if (hasOperation(context.operationManager)) setReadonly();
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getRepeatComment(comments)!;
    await editRepeat(comment.nameorid!, comment.folderid!);
    reload();
  });

  return { el, update };
}
