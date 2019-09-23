import { TEXT } from "@/common/lang";
import context from "@/common/context";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { createDataEdtor } from "@/components/dataEditor";
import { getEditableData } from "@/components/dataEditor/utils";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { setGuid, clearKoobooInfo, markDirty } from "@/kooboo/utils";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getEditComment } from "../utils";
import { operationRecord } from "@/operation/Record";

export default class EditDataItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT_DATA);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let editableData = getEditableData(element);
    if (!editableData) return this.setVisiable(false);
    let comments = KoobooComment.getComments(editableData.cleanParent);
    if (!getEditComment(comments)) return this.setVisiable(false);
  }

  async click() {
    this.parentMenu.hidden();
    let { element } = context.lastSelectedDomEventArgs;
    let { cleanParent, koobooId, list } = getEditableData(element)!;
    let comments = KoobooComment.getComments(cleanParent);
    let comment = getEditComment(comments)!;
    let startContent = cleanParent.innerHTML;
    try {
      await createDataEdtor(list);
      let value = clearKoobooInfo(cleanParent.innerHTML);
      if (value == clearKoobooInfo(startContent)) return;
      let guid = setGuid(cleanParent);
      markDirty(cleanParent);
      let units = [new InnerHtmlUnit(startContent)];
      let logs = [DomLog.createUpdate(comment.nameorid!, value, koobooId!, comment.objecttype!)];
      let operation = new operationRecord(units, logs, guid);
      context.operationManager.add(operation);
    } catch (error) {
      cleanParent.innerHTML = startContent;
    }
  }
}
