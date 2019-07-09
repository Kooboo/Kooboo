import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isDynamicContent } from "@/kooboo/utils";
import { isLink } from "@/dom/utils";
import { getViewComment, getUrlComment, updateDomLink, updateUrlLink, updateAttributeLink } from "../utils";

export function createEditLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;

    if (!isLink(args.element)) setVisiable(false);
    if (!getViewComment(args.koobooComments)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let urlComment = getUrlComment(args.koobooComments);
    let viewComment = getViewComment(args.koobooComments)!;
    if (args.cleanElement) {
      updateDomLink(args.cleanElement, args.cleanKoobooId!, args.element, viewComment);
    } else if (urlComment) {
      updateUrlLink(args.element, args.koobooId!, urlComment, viewComment);
    } else {
      updateAttributeLink(args.element, args.koobooId!, viewComment);
    }
  });

  return { el, update };
}
