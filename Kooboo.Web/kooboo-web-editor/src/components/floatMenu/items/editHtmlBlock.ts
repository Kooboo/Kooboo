import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getHtmlBlockComment, hasOperation } from "../utils";
import { reload } from "@/dom/utils";
import { editHtmlBlock } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createEditHtmlBlockItem(): MenuItem {
  const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_HTML_BLOCK, MenuActions.editHtmlBlock);

  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    if (!getHtmlBlockComment(comments)) return setVisiable(false);
    if (hasOperation(context.operationManager)) {
      setReadonly();
    }
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getHtmlBlockComment(comments)!;
    let nameorid = comment.nameorid;
    await editHtmlBlock(nameorid!);
    reload();
  });

  return { el, update };
}
