import { createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getRepeatComment, hasOperation } from "../utils";
import { reload, getAllNode } from "@/dom/utils";
import { editRepeat } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { OBJECT_TYPE } from "@/common/constants";
import { isSingleCommentWrap, previousComment } from "@/kooboo/utils";

export function createEditRepeatItem() {
  const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_REPEAT, MenuActions.editRepeat);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (!getRepeatComment(comments) && !getRelatedRepeatComment(args.element)) setVisiable(false);
    if (hasOperation(context.operationManager)) setReadonly();
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getRepeatComment(comments);
    if (!comment) comment = getRelatedRepeatComment(args.element);
    await editRepeat(comment!.nameorid!, comment!.folderid!);
    reload();
  });

  return { el, update };
}

function getRelatedRepeatComment(el: HTMLElement) {
  let comment = previousComment(el);
  if (!comment || !KoobooComment.isComment(comment) || !isSingleCommentWrap(comment)) return;
  let koobooComment = new KoobooComment(comment);
  if (koobooComment.end) return;
  for (const i of getAllNode(document.body)) {
    if (i instanceof Comment && KoobooComment.isComment(i) && !KoobooComment.isEndComment(i)) {
      let c = new KoobooComment(i);
      if (c.objecttype == OBJECT_TYPE.contentrepeater && c.nameorid == koobooComment.nameorid) {
        return c;
      }
    }
  }
}
