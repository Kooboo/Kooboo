import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, isDynamicContent, getCleanParent, isDirty, markDirty, getRelatedRepeatComment } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { Log } from "@/operation/recordLogs/Log";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { getViewComment, getRepeatComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";

export default class DeleteItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.DELETE);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let { parent } = getCleanParent(args.element);
    if (!args.koobooId) return this.setVisiable(false);
    let comment = getViewComment(comments);
    if (!comment) return this.setVisiable(false);
    if (getRepeatComment(comments)) return this.setVisiable(false);
    if (getRelatedRepeatComment(args.element)) return this.setVisiable(false);
    if (isDirty(args.element) && parent && isDynamicContent(parent)) return this.setVisiable(false);
    if (isBody(args.element)) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let { koobooId, parent } = getCleanParent(args.element);
    let comments = KoobooComment.getComments(args.element);
    let comment = getViewComment(comments)!;
    parent = parent || args.element.parentElement!;
    let oldValue = parent.innerHTML;
    let guid = setGuid(parent);
    args.element.parentElement!.removeChild(args.element);
    let log!: Log;
    markDirty(parent);
    if (isDirty(args.element) && parent) {
      log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(parent.innerHTML), koobooId!, comment.objecttype!);
    } else {
      log = DomLog.createDelete(comment.nameorid!, args.koobooId!, comment.objecttype!);
    }
    let operation = new operationRecord([new InnerHtmlUnit(oldValue)], [log], guid);
    context.operationManager.add(operation);
  }
}
