import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isDynamicContent, clearKoobooInfo, getCleanParent, isDirty, markDirty, setGuid } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { setInlineEditor } from "@/components/richEditor";
import { getMenuComment, getEditComment, clearContent, isEditable } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { Log } from "@/operation/recordLogs/Log";
import { OBJECT_TYPE } from "@/common/constants";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { LabelLog } from "@/operation/recordLogs/LabelLog";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class EditItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) return this.setVisiable(false);
    if (getMenuComment(comments)) return this.setVisiable(false);
    if (!getEditComment(comments)) return this.setVisiable(false);
    if (!args.koobooId) return this.setVisiable(false);
    if (!isEditable(args.element)) return this.setVisiable(false);
    let { parent } = getCleanParent(args.element);
    if (isDirty(args.element) && parent && isDynamicContent(parent)) return this.setVisiable(false);
    if (isDynamicContent(args.element)) return this.setVisiable(false);
  }

  click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let startContent = element.innerHTML;
    const onSave = () => {
      if (clearContent(startContent) == clearContent(element.innerHTML)) return;
      let { koobooId: parentKoobooId, parent } = getCleanParent(element);
      let comments = KoobooComment.getComments(element);
      let dirtyEl = parent && isDirty(element) ? parent : element;
      markDirty(dirtyEl);
      koobooId = parentKoobooId && isDirty(element) ? parentKoobooId : koobooId;
      let guid = setGuid(element);
      let units = [new InnerHtmlUnit(startContent)];
      let comment = getEditComment(comments)!;
      let log: Log;

      if (comment.objecttype == OBJECT_TYPE.content) {
        log = ContentLog.createUpdate(comment.nameorid!, comment.fieldname!, clearKoobooInfo(element.innerHTML));
      } else if (comment.objecttype == OBJECT_TYPE.Label) {
        log = LabelLog.createUpdate(comment.bindingvalue!, clearKoobooInfo(element.innerHTML));
      } else {
        log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(dirtyEl.innerHTML), koobooId!, comment.objecttype!);
      }

      let operation = new operationRecord(units, [log], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      element.innerHTML = startContent;
    };
    setInlineEditor({ selector: element, onSave, onCancel });
  }
}
