import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import {
  setGuid,
  clearKoobooInfo,
  isDynamicContent,
  getGuidComment
} from "@/kooboo/utils";
import {
  getEditComment,
  getDelete,
  getMenu,
  getForm,
  getHtmlBlock
} from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { Log } from "@/operation/recordLogs/Log";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { OBJECT_TYPE } from "@/common/constants";
import { LabelLog } from "@/operation/recordLogs/LabelLog";

export function createDeleteItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (getMenu(args.koobooComments)) visiable = false;
    if (getForm(args.koobooComments)) visiable = false;
    if (getHtmlBlock(args.koobooComments)) visiable = false;
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
    if (args.closeParent) {
      let comment = getDelete(args.koobooComments)!;
      log = DomLog.createUpdate(
        comment.nameorid!,
        clearKoobooInfo(args.closeParent.innerHTML),
        args.parentKoobooId!,
        comment.objecttype!
      );
    } else {
      let comment = getEditComment(args.koobooComments)!;
      if (comment.objecttype == OBJECT_TYPE.content) {
        log = ContentLog.createDelete(comment.nameorid!);
      } else if (comment.objecttype == OBJECT_TYPE.label) {
        log = LabelLog.createDelete(comment.bindingvalue!, comment.objecttype!);
      }
    }

    let operation = new operationRecord(
      [new DeleteUnit(startContent)],
      [log],
      guid
    );

    context.operationManager.add(operation);
  });

  return {
    el,
    update
  };
}
