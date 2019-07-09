import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, isDynamicContent } from "@/kooboo/utils";
import { isLink } from "@/dom/utils";
import { getViewComment, getUrlComment, updateDomLink, updateUrlLink, updateAttributeLink } from "../utils";
import { createLinkPicker } from "@/components/linkPicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";

export function createEditLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);

  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;

    if (!isLink(args.element)) visiable = false;
    if (!getViewComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let urlComment = getUrlComment(args.koobooComments);
    let viewComment = getViewComment(args.koobooComments)!;
    if (args.closeParent) {
      updateDomLink(args.closeParent, args.parentKoobooId!, args.element, viewComment);
    } else if (urlComment) {
      updateUrlLink(args.element, args.koobooId!, urlComment, viewComment);
    } else {
      updateAttributeLink(args.element, args.koobooId!, viewComment);
    }
  });

  return { el, update };
}
