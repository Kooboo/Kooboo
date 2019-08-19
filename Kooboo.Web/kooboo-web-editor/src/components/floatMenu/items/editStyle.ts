import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { createStyleEditor } from "@/components/styleEditor";
import { setGuid, isDirty, clearKoobooInfo, getCleanParent } from "@/kooboo/utils";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { operationRecord } from "@/operation/Record";
import { getMenuComment, getHtmlBlockComment, getViewComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { isImg } from "@/dom/utils";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditStyleItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT_STYLE);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (isImg(args.element)) return this.setVisiable(false);
    if (getMenuComment(comments)) return this.setVisiable(false);
    if (getHtmlBlockComment(comments)) return this.setVisiable(false);
    if (!getViewComment(comments)) return this.setVisiable(false);
    if (!args.koobooId) return this.setVisiable(false);
  }

  async click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    const startContent = args.element.getAttribute("style");
    let comment = getViewComment(comments)!;
    try {
      let logs = await createStyleEditor(args.element, comment.nameorid!, comment.objecttype!, args.koobooId!);
      let guid = setGuid(args.element);
      let unit = new AttributeUnit(startContent!, "style");
      if (logs.length == 0) return;
      if (isDirty(args.element)) {
        logs = [DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(parent!.innerHTML), koobooId!, comment.objecttype!)];
      }
      let record = new operationRecord([unit], logs, guid);
      context.operationManager.add(record);
    } catch (error) {
      args.element.setAttribute("style", startContent!);
    }
  }
}
