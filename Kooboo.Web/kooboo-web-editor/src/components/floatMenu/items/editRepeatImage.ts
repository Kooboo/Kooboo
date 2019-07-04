import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getRepeatAttribute } from "../utils";
import { isDynamicContent, setGuid } from "@/kooboo/utils";
import { operationRecord } from "@/operation/Record";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";

export function createEditRepeatImageItem(): MenuItem {
  const { el, setVisiable } = createItem(
    TEXT.EDIT_IMAGE,
    MenuActions.editImage
  );
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) visiable = false;
    let comment = getRepeatAttribute(args.koobooComments);
    if (!comment || !comment.fieldname || comment.attributename != "src") {
      visiable = false;
    }
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getRepeatAttribute(args.koobooComments)!;
    let img = args.element as HTMLImageElement;
    let startContent = img.src;
    pickImg(path => {
      img.src = path;
      let guid = setGuid(img);
      let value = img.src;
      let unit = new AttributeUnit(startContent, "src");
      let log = ContentLog.createUpdate(
        comment.nameorid!,
        comment.fieldname!,
        value
      );
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    });
  });

  return { el, update };
}
