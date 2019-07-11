import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, isDynamicContent, getGuidComment, getCleanParent } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { Log } from "@/operation/recordLogs/Log";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { getViewComment, getFirstComment, isEditComment, getRepeatComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import createDiv from "@/dom/div";

export function createDeleteItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let firstComment = getFirstComment(comments);
    if (!firstComment || !isEditComment(firstComment)) setVisiable(false);

    if (getRepeatComment(comments)) setVisiable(false);
    if (isBody(args.element)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let { koobooId, parent } = getCleanParent(args.element);
    let comments = KoobooComment.getComments(args.element);
    let guid = setGuid(args.element);
    let guidComment = getGuidComment(guid);
    let startContent = args.element.outerHTML;
    let temp = createDiv();
    args.element.parentNode!.replaceChild(temp, args.element);
    temp.outerHTML = guidComment;
    let log!: Log;

    if (parent) {
      let comment = getViewComment(comments)!;
      log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(parent.innerHTML), koobooId!, comment.objecttype!);
    } else {
      let comment = getViewComment(comments)!;
      log = DomLog.createDelete(comment.nameorid!, args.koobooId!, comment.objecttype!);
    }

    let operation = new operationRecord([new DeleteUnit(startContent)], [log], guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}
