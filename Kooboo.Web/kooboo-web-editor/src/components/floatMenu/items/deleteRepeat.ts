import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeatSourceComment } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationRecord } from "@/operation/Record";
import { DeleteRepeatUnit } from "@/operation/recordUnits/DeleteRepeatUnit";
import { createDiv } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";
import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";

export default class DeleteRepeatItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.DELETE_REPEAR);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    if (isBody(element)) return this.setVisiable(false);
    let { nodes, startNode } = getWrapDom(element, "repeatitem");
    let comments = KoobooComment.getInnerComments(nodes);
    if (!startNode || !getRepeatSourceComment(comments)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let { nodes, startNode } = getWrapDom(element, "repeatitem");
    let comments = KoobooComment.getInnerComments(nodes);
    let repeatSourceComment = getRepeatSourceComment(comments)!;
    let guidComment = getGuidComment(repeatSourceComment.id);
    let temp = createDiv();
    startNode!.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    let oldValue = temp.innerHTML;
    temp.outerHTML = guidComment;
    let units = [new DeleteRepeatUnit(oldValue)];
    let log = new Log([...repeatSourceComment.infos, kvInfo.delete]);
    let operation = new operationRecord(units, [log], repeatSourceComment.id);
    context.operationManager.add(operation);
    emitHoverEvent(document.body);
    emitSelectedEvent();
  }
}
