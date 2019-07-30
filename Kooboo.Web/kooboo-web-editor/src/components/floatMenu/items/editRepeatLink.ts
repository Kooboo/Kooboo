import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isLink } from "@/dom/utils";
import { getAttributeComment, updateAttributeLink } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createEditRepeatLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);
  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!isLink(args.element)) return setVisiable(false);
    let comment = getAttributeComment(comments, "href");
    if (!comment || !comment.fieldname) return setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getAttributeComment(comments, "href");
    updateAttributeLink(args.element, args.koobooId!, comment!);
  });

  return { el, update };
}