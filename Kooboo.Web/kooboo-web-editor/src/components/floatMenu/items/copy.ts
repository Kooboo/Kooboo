import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, isDynamicContent, getGuidComment, getCleanParent, getRelatedRepeatComment } from "@/kooboo/utils";
import { TEXT } from "@/common/lang";
import { getEditComment, getRepeatComment, getViewComment } from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";

export default class CopyItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.COPY);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
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
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    let guid = setGuid(parent!);
    let oldValue = parent!.innerHTML;
    args.element.parentElement!.insertBefore(cloneElement, args.element.nextSibling);
    markDirty(parent!);
    let value = clearKoobooInfo(parent!.innerHTML);
    let unit = new InnerHtmlUnit(oldValue);
    let comment = getEditComment(comments)!;
    let log = DomLog.createUpdate(comment.nameorid!, value, koobooId!, comment.objecttype!);

    let operation = new operationRecord([unit], [log], guid);
    context.operationManager.add(operation);
  }
}
