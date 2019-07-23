import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeatComment, changeNameOrId } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { CopyRepeatUnit } from "@/operation/recordUnits/CopyRepeatUnit";
import { operationRecord } from "@/operation/Record";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createCopyRepeatItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.COPY_REPEAT, MenuActions.copyRepeat);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (isBody(args.element)) return setVisiable(false);
    if (!getRepeatComment(comments)) return setVisiable(false);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let { nodes } = getWrapDom(args.element, OBJECT_TYPE.contentrepeater);
    if (!nodes || nodes.length == 0) return;
    let anchor: Node = nodes[nodes.length - 1];
    let parent = anchor.parentNode!;
    let guid = newGuid() + "_name";
    for (const node of nodes.reverse()) {
      let insertNode = node.cloneNode(true);
      changeNameOrId(insertNode, guid);
      parent.insertBefore(insertNode, anchor.nextSibling);
    }
    let comment = getRepeatComment(comments);
    let units = [new CopyRepeatUnit(getGuidComment(guid))];
    let logs = [ContentLog.createCopy(guid, comment!.nameorid!)];

    let operation = new operationRecord(units, logs, guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}
