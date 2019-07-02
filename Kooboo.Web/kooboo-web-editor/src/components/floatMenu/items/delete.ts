import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { KOOBOO_GUID, ACTION_TYPE, EDITOR_TYPE } from "@/common/constants";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import {
  setGuid,
  markDirty,
  clearKoobooInfo,
  isDynamicContent,
  getGuidComment
} from "@/kooboo/utils";
import { getEditComment, getDeleteComment } from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { InnerHtmlLog } from "@/operation/recordLogs/InnerHtmlLog";
import { Log } from "@/operation/recordLogs/Log";
import { DeleteLog } from "@/operation/recordLogs/DeleteLog";

export function createDeleteItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
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

    let log: Log;
    if (args.closeParent) {
      let comment = getEditComment(args.koobooComments);
      log = new InnerHtmlLog(comment!, args.parentKoobooId!, args.closeParent!);
    } else {
      let comment = getDeleteComment(args.koobooComments);
      log = new DeleteLog(comment!, args.koobooId!);
    }

    let operation = new operationRecord(
      [new DeleteUnit(startContent)],
      [log],
      guid
    );

    context.operationManager.add(operation);
    // if (args.closeParent) {
    //   updateDom(args);
    // } else if (args.element.parentElement) {
    //   deleteDom(args);
    // }
  });

  return {
    el,
    update
  };
}

// const updateDom = (args: SelectedDomEventArgs) => {
//   setGuid(args.closeParent!);
//   let startContent = args.closeParent!.innerHTML;
//   args.element.parentElement!.removeChild(args.element);
//   markDirty(args.closeParent!);
//   let endContent = args.closeParent!.innerHTML;
//   let operation = new Operation(
//     args.closeParent!.getAttribute(KOOBOO_GUID)!,
//     startContent,
//     endContent,
//     args.koobooComments[0],
//     args.parentKoobooId,
//     ACTION_TYPE.update,
//     clearKoobooInfo(args.closeParent!.innerHTML),
//     EDITOR_TYPE.dom
//   );
//   context.operationManager.add(operation);
// };

// const deleteDom = (args: SelectedDomEventArgs) => {
//   let parentElement = args.element.parentElement!;
//   setGuid(parentElement);
//   let startContent = parentElement.innerHTML;
//   parentElement.removeChild(args.element);
//   let endContent = parentElement.innerHTML;
//   let operation = new Operation(
//     parentElement.getAttribute(KOOBOO_GUID)!,
//     startContent,
//     endContent,
//     getEditComment(args.koobooComments)!,
//     args.koobooId,
//     ACTION_TYPE.delete,
//     "",
//     EDITOR_TYPE.dom
//   );
//   context.operationManager.add(operation);
// };
