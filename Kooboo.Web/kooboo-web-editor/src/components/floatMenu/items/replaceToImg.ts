import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getViewComment, getRepeatComment } from "../utils";
import { isDynamicContent, setGuid, markDirty, clearKoobooInfo, getCleanParent, getRelatedRepeatComment } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createImg } from "@/dom/element";

export function createReplaceToImgItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.REPLACE_TO_IMG, MenuActions.replaceToImg);
  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (getRepeatComment(comments)) return setVisiable(false);
    if (getRelatedRepeatComment(args.element)) return setVisiable(false);
    if (!getViewComment(comments)) return setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent && !koobooId) return setVisiable(false);
    if (isImg(args.element)) return setVisiable(false);
    if (parent && isDynamicContent(parent)) return setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    setGuid(parent!);
    let startContent = parent!.innerHTML;
    try {
      let style = JSON.parse(JSON.stringify(getComputedStyle(args.element)));
      let img = createImg();
      img.setAttribute(KOOBOO_ID, args.koobooId!);
      args.element.parentElement!.replaceChild(img, args.element);
      img.style.width = style.width;
      img.style.height = style.height;
      img.style.display = style.display;
      await createImagePicker(img);
      markDirty(parent!);
      let guid = setGuid(parent!);
      let value = clearKoobooInfo(parent!.innerHTML);
      let comment = getViewComment(comments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = DomLog.createUpdate(comment.nameorid!, value, koobooId!, comment.objecttype!);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      parent!.innerHTML = startContent;
    }
  });

  return { el, update };
}
