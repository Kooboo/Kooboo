import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeat, changeNameOrId } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { CopyRepeatUnit } from "@/operation/recordUnits/CopyRepeatUnit";
import { CopyRepeatLog } from "@/operation/recordLogs/CopyRepeatLog";
import { operationRecord } from "@/operation/Record";

export function createCopyRepeatItem(): MenuItem {
  const { el, setVisiable } = createItem(
    TEXT.COPY_REPEAT,
    MenuActions.copyRepeat
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

    let units = [new CopyRepeatUnit(getGuidComment(guid))];
    let logs = [new CopyRepeatLog(getRepeat(args.koobooComments)!, guid)];

    let operation = new operationRecord(units, logs, guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}
