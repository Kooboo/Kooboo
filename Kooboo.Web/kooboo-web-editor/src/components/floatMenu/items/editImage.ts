import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment } from "../utils";
import { isDynamicContent, setGuid, markDirty, clearKoobooInfo } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { DomLog } from "@/operation/recordLogs/DomLog";

export function createEditImageItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_IMAGE, MenuActions.editImage);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) visiable = false;
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
    try {
      await createImagePicker(args.element as HTMLImageElement);
      if (startContent == args.closeParent.innerHTML) return;
      markDirty(args.closeParent);
      let guid = setGuid(args.closeParent);
      let value = clearKoobooInfo(args.closeParent!.innerHTML);
      let comment = getEditComment(args.koobooComments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = DomLog.createUpdate(comment.nameorid!, value, args.parentKoobooId!, comment.objecttype!);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      args.closeParent.innerHTML = startContent;
    }
  });

  return { el, update };
}
