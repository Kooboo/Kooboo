import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, isDynamicContent, getGuidComment } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { Log } from "@/operation/recordLogs/Log";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { getViewComment, getFirstComment, isEditComment, getRepeatComment } from "../utils";

export function createDeleteItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let firstComment = getFirstComment(args.koobooComments);
    if (!firstComment || !isEditComment(firstComment)) setVisiable(false);

    if (getRepeatComment(args.koobooComments)) setVisiable(false);
    if (isBody(args.element)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let guid = setGuid(args.element);
    let guidComment = getGuidComment(guid);
    let startContent = args.element.outerHTML;
    let temp = document.createElement("div");
    args.element.parentNode!.replaceChild(temp, args.element);
    temp.outerHTML = guidComment;
    let log!: Log;
    if (args.cleanElement) {
      let comment = getViewComment(args.koobooComments)!;
      log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(args.cleanElement.innerHTML), args.cleanKoobooId!, comment.objecttype!);
    } else {
      let comment = getViewComment(args.koobooComments)!;
      log = DomLog.createDelete(comment.nameorid!, args.koobooId!, comment.objecttype!);
    }

    let operation = new operationRecord([new DeleteUnit(startContent)], [log], guid);

    context.operationManager.add(operation);
  });

  return {
    el,
    update
  };
}
