import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, getGuidComment, getWrapDom } from "@/kooboo/utils";
import { operationRecord } from "@/operation/Record";
import { DeleteUnit } from "@/operation/recordUnits/DeleteUnit";
import { Log } from "@/operation/recordLogs/Log";
import { getHtmlBlockComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { HtmlblockLog } from "@/operation/recordLogs/HtmlblockLog";
import { OBJECT_TYPE } from "@/common/constants";
import { createDiv } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";

export function createDeleteHtmlBlockItem(): MenuItem {
  return new DeleteHtmlBlockItem();
}

class DeleteHtmlBlockItem extends BaseMenuItem {
  constructor() {
    super();

    const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    if (!getHtmlBlockComment(comments)) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let guid = setGuid(args.element);
    let guidComment = getGuidComment(guid);
    let startContent = args.element.outerHTML;
    let { nodes } = getWrapDom(args.element, OBJECT_TYPE.htmlblock);
    let temp = createDiv();
    args.element.parentNode!.replaceChild(temp, args.element);
    temp.outerHTML = guidComment;
    let log!: Log;
    let htmlblockComment = getHtmlBlockComment(comments)!;
    nodes = nodes.filter(f => f != args.element);
    if (nodes.some(s => s instanceof HTMLElement)) {
      let temp = createDiv();
      nodes.forEach(i => temp.appendChild(i.cloneNode(true)));
      log = HtmlblockLog.createUpdate(htmlblockComment.nameorid!, clearKoobooInfo(temp.innerHTML));
    } else {
      log = HtmlblockLog.createDelete(htmlblockComment.nameorid!);
    }
    let operation = new operationRecord([new DeleteUnit(startContent)], [log], guid);
    context.operationManager.add(operation);
  }
}
