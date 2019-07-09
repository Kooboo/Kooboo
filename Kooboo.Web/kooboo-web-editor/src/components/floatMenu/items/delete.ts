import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, isDynamicContent, getGuidComment } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { Log } from "@/operation/recordLogs/Log";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { OBJECT_TYPE } from "@/common/constants";
import { LabelLog } from "@/operation/recordLogs/LabelLog";
import { getMenuComment, getFormComment, getHtmlBlockComment, getEditComment, getViewComment } from "../utils";

export function createDeleteItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (getMenuComment(args.koobooComments)) visiable = false;
    if (getFormComment(args.koobooComments)) visiable = false;
    if (getHtmlBlockComment(args.koobooComments)) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
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
      let comment = getEditComment(args.koobooComments)!;
      if (comment.objecttype == OBJECT_TYPE.content) {
        log = ContentLog.createDelete(comment.nameorid!);
      } else if (comment.objecttype == OBJECT_TYPE.Label) {
        log = LabelLog.createDelete(comment.bindingvalue!);
      } else {
        log = DomLog.createDelete(comment.nameorid!, args.koobooId!, comment.objecttype!);
      }
    }

    let operation = new operationRecord([new DeleteUnit(startContent)], [log], guid);

    context.operationManager.add(operation);
  });

  return {
    el,
    update
  };
}
