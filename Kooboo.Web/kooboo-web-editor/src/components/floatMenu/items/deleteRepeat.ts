import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeatItemId } from "../utils";
import { getWrapDom, getGuidComment } from "@/kooboo/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationRecord } from "@/operation/Record";
import { DeleteRepeatUnit } from "@/operation/recordUnits/DeleteRepeatUnit";
import { createDiv } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";

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

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    if (isBody(element)) return this.setVisiable(false);
    if (!getRepeatItemId(comments)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let { nodes, startNode } = getWrapDom(element, "repeatitem");
    if (!nodes || nodes.length == 0 || !startNode) return;
    let comments = KoobooComment.getComments(element);
    let comment = comments.find(f => f.source == "repeatitem")!;
    let id = getRepeatItemId(comments)!;
    let guidComment = getGuidComment(id);
    let temp = createDiv();
    startNode!.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    let oldValue = temp.innerHTML;
    temp.outerHTML = guidComment;

    let units = [new DeleteRepeatUnit(oldValue)];
    let log = new Log([...comment.infos, kvInfo.value(id), kvInfo.delete]);

    let operation = new operationRecord(units, [log], id);
    context.operationManager.add(operation);
  }
}
