import { createItem, MenuItem } from "../basic";
import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, isDynamicContent, getGuidComment, isDirty, getCleanParent } from "@/kooboo/utils";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { TEXT } from "@/common/lang";
import { getEditComment, getFirstComment, isViewComment } from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { CopyUnit } from "@/operation/recordUnits/CopyUnit";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createCopyItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.COPY, MenuActions.copy);

  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let firstComment = getFirstComment(comments);
    if (!firstComment || !isViewComment(firstComment)) return setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent && !koobooId) return setVisiable(false);
    if (isBody(args.element)) return setVisiable(false);
    if (parent && isDynamicContent(parent)) return setVisiable(false);
  };

  el.addEventListener("click", e => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    let guid = setGuid(cloneElement, newGuid());
    args.element.parentElement!.insertBefore(cloneElement, args.element.nextSibling);

    markDirty(parent!);
    let value = clearKoobooInfo(parent!.innerHTML);
    let units = [new CopyUnit(getGuidComment(guid))];
    let comment = getEditComment(comments)!;
    let log = DomLog.createUpdate(comment.nameorid!, value, koobooId!, comment.objecttype!);

    let operation = new operationRecord(units, [log], guid);
    context.operationManager.add(operation);
  });

  return {
    el,
    update
  };
}
