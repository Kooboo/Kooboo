import { createItem, MenuItem } from "../basic";
import context from "@/common/context";
import {
  setGuid,
  markDirty,
  clearKoobooInfo,
  isDynamicContent,
  getGuidComment
} from "@/kooboo/koobooUtils";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { TEXT } from "@/common/lang";
import { getEditComment } from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { CopyUnit } from "@/operation/recordUnits/CopyUnit";
import { InnerHtmlLog } from "@/operation/recordLogs/InnerHtmlLog";

export function createCopyItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.COPY, MenuActions.copy);

  const update = () => {
    var visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (!args.closeParent || !args.parentKoobooId) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", e => {
    let args = context.lastSelectedDomEventArgs;

    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    let guid = setGuid(cloneElement, true);
    args.element.parentElement!.insertBefore(
      cloneElement,
      args.element.nextSibling
    );

    markDirty(args.closeParent!);

    let units = [new CopyUnit(getGuidComment(guid))];
    let comment = getEditComment(args.koobooComments)!;
    let logs = [
      new InnerHtmlLog(comment, args.parentKoobooId!, args.closeParent!)
    ];

    let operation = new operationRecord(units, logs, guid);
    context.operationManager.add(operation);
  });

  return {
    el,
    update
  };
}
