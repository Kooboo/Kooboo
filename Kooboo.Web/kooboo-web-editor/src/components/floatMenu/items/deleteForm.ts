import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { setGuid, getGuidComment } from "@/kooboo/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { Log } from "@/operation/recordLogs/Log";
import { getFormComment, getViewComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import { DomLog } from "@/operation/recordLogs/DomLog";

export function createDeleteFormItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (!getFormComment(comments)) return setVisiable(false);
    if (!getViewComment(comments)) return setVisiable(false);
    if (!args.koobooId) return setVisiable(false);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let guid = setGuid(args.element);
    let guidComment = getGuidComment(guid);
    let startContent = args.element.outerHTML;
    let temp = createDiv();
    args.element.parentNode!.replaceChild(temp, args.element);
    temp.outerHTML = guidComment;
    let log!: Log;
    let comment = getViewComment(comments)!;
    log = DomLog.createDelete(comment.nameorid!, args.koobooId!, comment.objecttype!);
    let operation = new operationRecord([new DeleteUnit(startContent)], [log], guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}
