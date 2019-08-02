import { createItem, MenuItem } from "../basic";
import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, isDynamicContent, getGuidComment, getCleanParent, getRelatedRepeatComment } from "@/kooboo/utils";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { TEXT } from "@/common/lang";
import { getEditComment, getRepeatComment, getViewComment } from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { CopyUnit } from "@/operation/recordUnits/CopyUnit";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class CopyItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = createItem(TEXT.COPY, MenuActions.copy);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (getRepeatComment(comments)) return this.setVisiable(false);
    if (getRelatedRepeatComment(args.element)) return this.setVisiable(false);
    if (!getViewComment(comments)) return this.setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent && !koobooId) return this.setVisiable(false);
    if (isBody(args.element)) return this.setVisiable(false);
    if (parent && isDynamicContent(parent)) return this.setVisiable(false);
  }

  click() {
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
  }
}
