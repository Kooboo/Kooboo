import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeatComment } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationRecord } from "@/operation/Record";
import { DeleteRepeatUnit } from "@/operation/recordUnits/DeleteRepeatUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";

export function createDeleteRepeatItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE_REPEAR, MenuActions.deleteRepeat);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (isBody(args.element)) setVisiable(false);
    if (!getRepeatComment(comments)) setVisiable(false);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let { nodes, startNode } = getWrapDom(args.element, OBJECT_TYPE.contentrepeater);
    if (!nodes || nodes.length == 0 || !startNode) return;

    let comment = new KoobooComment(startNode);
    let guid = comment.nameorid!;
    let guidComment = getGuidComment(guid);
    let temp = document.createElement("div");
    startNode!.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    let oldValue = temp.innerHTML;
    temp.outerHTML = guidComment;

    let units = [new DeleteRepeatUnit(oldValue)];
    let logs = [ContentLog.createDelete(guid)];

    let operation = new operationRecord(units, logs, guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}
