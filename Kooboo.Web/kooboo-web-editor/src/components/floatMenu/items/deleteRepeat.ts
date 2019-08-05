import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeatComment } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationRecord } from "@/operation/Record";
import { DeleteRepeatUnit } from "@/operation/recordUnits/DeleteRepeatUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { createDiv } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

export default class DeleteRepeatItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = createItem(TEXT.CLICK);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) return this.setVisiable(false);
    if (!getRepeatComment(comments)) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    
    let { nodes, startNode } = getWrapDom(args.element, OBJECT_TYPE.contentrepeater);
    if (!nodes || nodes.length == 0 || !startNode) return;

    let comment = new KoobooComment(startNode);
    let guid = comment.nameorid!;
    let guidComment = getGuidComment(guid);
    let temp = createDiv();
    startNode!.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    let oldValue = temp.innerHTML;
    temp.outerHTML = guidComment;

    let units = [new DeleteRepeatUnit(oldValue)];
    let logs = [ContentLog.createDelete(guid)];

    let operation = new operationRecord(units, logs, guid);
    context.operationManager.add(operation);
  }
}
