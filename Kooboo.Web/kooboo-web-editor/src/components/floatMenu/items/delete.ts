import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import {
  setGuid,
  clearKoobooInfo,
  isDynamicContent,
  getGuidComment,
  getCleanParent,
  isDirty,
  markDirty,
  getRelatedRepeatComment
} from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { Log } from "@/operation/recordLogs/Log";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { getViewComment, getRepeatComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";

export function createDeleteItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let { parent } = getCleanParent(args.element);
    if (!args.koobooId) return setVisiable(false);
    let comment = getViewComment(comments);
    if (!comment) return setVisiable(false);
    if (getRepeatComment(comments)) return setVisiable(false);
    if (getRelatedRepeatComment(args.element)) return setVisiable(false);
    if (isDirty(args.element) && parent && isDynamicContent(parent)) return setVisiable(false);
    if (isBody(args.element)) return setVisiable(false);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let { koobooId, parent } = getCleanParent(args.element);
    let comments = KoobooComment.getComments(args.element);
    let comment = getViewComment(comments)!;
    let guid = setGuid(args.element);
    let guidComment = getGuidComment(guid);
    let startContent = args.element.outerHTML;
    let temp = createDiv();
    args.element.parentNode!.replaceChild(temp, args.element);
    temp.outerHTML = guidComment;
    let log!: Log;
    if (isDirty(args.element) && parent) {
      markDirty(parent);
      log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(parent.innerHTML), koobooId!, comment.objecttype!);
    } else {
      log = DomLog.createDelete(comment.nameorid!, args.koobooId!, comment.objecttype!);
    }
    let operation = new operationRecord([new DeleteUnit(startContent)], [log], guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}
