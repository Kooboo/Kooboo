import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isLink } from "@/dom/utils";
import { getRepeatAttribute } from "../utils";
import { isDynamicContent, setGuid } from "@/kooboo/utils";
import { operationRecord } from "@/operation/Record";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { createLinkPicker } from "@/components/linkPicker";

export function createEditRepeatLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (!isLink(args.element)) visiable = false;
    let comment = getRepeatAttribute(args.koobooComments);
    if (!comment || !comment.fieldname || comment.attributename != "href") {
      visiable = false;
    }
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getRepeatAttribute(args.koobooComments)!;
    let link = args.element as HTMLLinkElement;
    let startContent = link.href;
    try {
      link.href = await createLinkPicker(link.href);
      let guid = setGuid(link);
      let value = link.href;
      let unit = new AttributeUnit(startContent, "href");
      let log = ContentLog.createUpdate(
        comment.nameorid!,
        comment.fieldname!,
        value
      );
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      link.href = startContent;
    }
  });

  return { el, update };
}
