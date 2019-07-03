import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, isDynamicContent } from "@/kooboo/utils";
import { isLink } from "@/dom/utils";
import { getEditComment } from "../utils";
import { createLinkPicker } from "@/components/linkPicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";

export function createEditLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);

  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;

    if (!isLink(args.element)) visiable = false;
    if (!args.closeParent || !args.parentKoobooId) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    if (!args.closeParent) return false;
    setGuid(args.closeParent);
    let startContent = args.closeParent.innerHTML;
    let href = args.element.getAttribute("href")!;

    try {
      let url = await createLinkPicker(href);
      args!.element.setAttribute("href", url);
      let guid = setGuid(args.closeParent);
      let value = clearKoobooInfo(args.closeParent!.innerHTML);
      let comment = getEditComment(args.koobooComments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = DomLog.createUpdate(
        comment.nameorid!,
        value,
        args.parentKoobooId!,
        comment.objecttype!
      );
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      args.closeParent.innerHTML = startContent;
    }
  });

  return { el, update };
}
