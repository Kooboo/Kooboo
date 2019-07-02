import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isBody, getAllNode } from "@/dom/utils";
import { getRepeat } from "../utils";
import { getWrapDom, getGuidComment, setGuid } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { CopyRepeatUnit } from "@/operation/recordUnits/copyRepeatUnit";
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
    let guid = newGuid();
    for (const node of nodes.reverse()) {
      let insertNode = node.cloneNode(true);
      changeGuid(insertNode, guid);
      parent.insertBefore(insertNode, anchor.nextSibling);
    }

    let units = [new CopyRepeatUnit(getGuidComment(guid))];
    let logs = [
      new CopyRepeatLog(getRepeat(args.koobooComments)!, guid, args.koobooId!)
    ];

    let operation = new operationRecord(units, logs, guid);
    context.operationManager.add(operation);
  });

  return { el, update };
}

function changeGuid(node: Node, guid: string) {
  if (KoobooComment.isKoobooComment(node)) {
    node.nodeValue = node.nodeValue!.replace(
      /--nameorid='.{36}'/,
      `--nameorid='${guid}'`
    );
  }
  if (node instanceof HTMLElement) {
    for (const iterator of getAllNode(node)) {
      changeGuid(iterator, guid);
    }
  }
}
