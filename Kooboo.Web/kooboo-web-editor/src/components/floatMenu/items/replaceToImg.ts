import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getViewComment, isViewComment, getFirstComment } from "../utils";
import { isDynamicContent, setGuid, markDirty, clearKoobooInfo, getCleanParent } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createReplaceToImgItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.REPLACE_TO_IMG, MenuActions.replaceToImg);
  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let firstComment = getFirstComment(comments);
    if (!firstComment || !isViewComment(firstComment)) setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent || !koobooId) setVisiable(false);
    if (isImg(args.element)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    setGuid(parent!);
    let startContent = parent!.innerHTML;
    try {
      let style = getComputedStyle(args.element);
      let img = document.createElement("img");

      img.setAttribute(KOOBOO_ID, args.koobooId!);
      img.style.width = style.width;
      img.style.height = style.height;
      args.element.parentElement!.replaceChild(img, args.element);
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
