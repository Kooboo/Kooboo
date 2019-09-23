import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg, isInTable } from "@/dom/utils";
import { getRepeatComment, getViewComment, getEditComment, clearContent } from "../utils";
import { isDynamicContent, getCleanParent, getRelatedRepeatComment, clearKoobooInfo, setGuid } from "@/kooboo/utils";
import { setInlineEditor } from "@/components/richEditor";
import { KOOBOO_ID, KOOBOO_DIRTY } from "@/common/constants";
import { emitSelectedEvent, emitHoverEvent } from "@/dom/events";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createP } from "@/dom/element";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { htmlModeCheck } from "@/common/utils";

export default class ReplaceToTextItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.REPLACE_TO_TEXT);
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
    if (isInTable(args.element)) return this.setVisiable(false);
    if (!getViewComment(comments)) return this.setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent || !koobooId) return this.setVisiable(false);
    if (!isImg(args.element)) return this.setVisiable(false);
    if (isDynamicContent(parent)) return this.setVisiable(false);
  }

  async click() {
    if (!htmlModeCheck()) {
      this.parentMenu.hidden();
      return;
    }

    let args = context.lastSelectedDomEventArgs;
    let { parent, koobooId } = getCleanParent(args.element);
    let startContent = parent!.innerHTML;
    let text = createP();
    let style = getComputedStyle(args.element);
    let width = style.width;
    let widthImportant = args.element.style.getPropertyPriority("width");
    let height = style.height;
    let heightImportant = args.element.style.getPropertyPriority("height");
    args.element.parentElement!.replaceChild(text, args.element);
    text.setAttribute(KOOBOO_ID, args.koobooId!);
    text.setAttribute(KOOBOO_DIRTY, "");
    text.style.setProperty("width", width, widthImportant);
    text.style.setProperty("height", height, heightImportant);
    text.style.display = "block";
    emitHoverEvent(text);
    emitSelectedEvent();

    const onSave = () => {
      if (clearContent(startContent) == clearContent(text.innerHTML)) return;
      let guid = setGuid(parent!);
      let comments = KoobooComment.getComments(parent!);
      let comment = getEditComment(comments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(parent!.innerHTML), koobooId!, comment.objecttype!);
      let operation = new operationRecord([unit], [log], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      parent!.innerHTML = startContent;
    };

    await setInlineEditor({ selector: text, onSave, onCancel });
    this.parentMenu.hidden();
  }
}
