import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isDynamicContent, getCleanParent } from "@/kooboo/utils";
import { isLink } from "@/dom/utils";
import { getViewComment, getUrlComment, updateDomLink, updateUrlLink, updateAttributeLink, getAttributeComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createEditLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (!isLink(args.element)) setVisiable(false);
    if (getAttributeComment(comments)) setVisiable(false);
    if (!getViewComment(comments)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let urlComment = getUrlComment(comments);
    let viewComment = getViewComment(comments)!;
    let { koobooId, parent } = getCleanParent(args.element);
    if (parent) {
      updateDomLink(parent, koobooId!, args.element, viewComment);
    } else if (urlComment) {
      updateUrlLink(args.element, args.koobooId!, urlComment, viewComment);
    } else {
      updateAttributeLink(args.element, args.koobooId!, viewComment);
    }
  });

  return { el, update };
}
