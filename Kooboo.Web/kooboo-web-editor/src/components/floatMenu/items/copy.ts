import { createItem, MenuItem } from "../basic";
import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, isDynamicContent, getGuidComment, getCloseElement } from "@/kooboo/utils";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { TEXT } from "@/common/lang";
import { getEditComment, getFirstComment, isViewComment } from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { CopyUnit } from "@/operation/recordUnits/CopyUnit";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KOOBOO_ID } from "@/common/constants";

export function createCopyItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.COPY, MenuActions.copy);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let firstComment = getFirstComment(args.koobooComments);
    if (!firstComment || !isViewComment(firstComment)) setVisiable(false);

    let closeParent = getCloseElement(args.element.parentElement!)!;
    let koobooId = closeParent.getAttribute(KOOBOO_ID);
    if (!closeParent || !koobooId) setVisiable(false);

    if (isBody(args.element)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", e => {
    let args = context.lastSelectedDomEventArgs;
    let closeParent = getCloseElement(args.element.parentElement!)!;
    let koobooId = closeParent.getAttribute(KOOBOO_ID)!;
    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    let guid = setGuid(cloneElement, newGuid());
    args.element.parentElement!.insertBefore(cloneElement, args.element.nextSibling);

    markDirty(closeParent);
    let value = clearKoobooInfo(closeParent.innerHTML);
    let units = [new CopyUnit(getGuidComment(guid))];
    let comment = getEditComment(args.koobooComments)!;
    let log = DomLog.createUpdate(comment.nameorid!, value, koobooId, comment.objecttype!);

    let operation = new operationRecord(units, [log], guid);
    context.operationManager.add(operation);
  });

  return {
    el,
    update
  };
}
