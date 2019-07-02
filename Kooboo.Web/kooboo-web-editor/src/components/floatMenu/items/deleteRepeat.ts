import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeat } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationRecord } from "@/operation/Record";
import { DeleteRepeatLog } from "@/operation/recordLogs/DeleteRepeatLog";
import { DeleteRepeatUnit } from "@/operation/recordUnits/DeleteRepeatUnit";

export function createDeleteRepeatItem(): MenuItem {
  const { el, setVisiable } = createItem(
    TEXT.DELETE_REPEAR,
    MenuActions.deleteRepeat
  );

  const update = () => {
    var visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (!getRepeat(args.koobooComments)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let { nodes, startNode } = getWrapDom(
      args.element,
      OBJECT_TYPE.contentrepeater
    );
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
    let logs = [new DeleteRepeatLog(getRepeat(args.koobooComments)!, guid)];

    let operation = new operationRecord(units, logs, guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}
