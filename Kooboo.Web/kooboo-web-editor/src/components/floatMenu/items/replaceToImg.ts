import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment } from "../utils";
import { isDynamicContent, setGuid, markDirty, clearKoobooInfo, getCloseElement } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KOOBOO_ID } from "@/common/constants";

export function createReplaceToImgItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.REPLACE_TO_IMG, MenuActions.replaceToImg);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    let closeParent = getCloseElement(args.element.parentElement!)!;
    let koobooId = closeParent.getAttribute(KOOBOO_ID);
    if (!closeParent || !koobooId) visiable = false;
    if (isImg(args.element)) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let closeParent = getCloseElement(args.element.parentElement!)!;
    let parentKoobooId = closeParent.getAttribute(KOOBOO_ID);
    let element = args.cleanElement ? args.cleanElement : closeParent;
    let koobooId = args.cleanKoobooId ? args.cleanKoobooId : parentKoobooId;
    setGuid(element);
    let startContent = element.innerHTML;
    try {
      let style = getComputedStyle(args.element);
      let img = document.createElement("img");

      img.setAttribute(KOOBOO_ID, args.koobooId!);
      img.style.width = style.width;
      img.style.height = style.height;
      args.element.parentElement!.replaceChild(img, args.element);
      await createImagePicker(img);
      markDirty(element);
      let guid = setGuid(element);
      let value = clearKoobooInfo(element.innerHTML);
      let comment = getEditComment(args.koobooComments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = DomLog.createUpdate(comment.nameorid!, value, koobooId!, comment.objecttype!);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      element.innerHTML = startContent;
    }
  });

  return { el, update };
}
