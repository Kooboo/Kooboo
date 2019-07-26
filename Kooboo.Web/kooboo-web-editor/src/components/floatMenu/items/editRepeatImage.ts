import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getAttributeComment } from "../utils";
import { setGuid } from "@/kooboo/utils";
import { operationRecord } from "@/operation/Record";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createEditRepeatImageItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_IMAGE, MenuActions.editImage);
  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) return setVisiable(false);
    let comment = getAttributeComment(comments, "src");
    if (!comment || !comment.fieldname) return setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getAttributeComment(comments, "src")!;
    let img = args.element as HTMLImageElement;
    let startContent = img.getAttribute("src")!;
    pickImg(path => {
      img.src = path;
      let guid = setGuid(img);
      let value = img.getAttribute("src")!;
      let unit = new AttributeUnit(startContent, "src");
      let log = ContentLog.createUpdate(comment.nameorid!, comment.fieldname!, value);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    });
  });

  return { el, update };
}
