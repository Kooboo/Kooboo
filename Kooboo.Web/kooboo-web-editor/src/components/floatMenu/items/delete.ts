import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, markDirty, getUnpollutedEl, isDynamicContent, getWrapDom, isDirty } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { getEditableComment, getRepeatSourceComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { KOOBOO_ID } from "@/common/constants";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";
import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";

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
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    let el = getUnpollutedEl(element);
    if (el && isBody(el)) return this.setVisiable(false);
    if (!el && (!KoobooComment.getAroundScopeComments(element) || !koobooId || isDirty(element))) return this.setVisiable(false);
    if (!getEditableComment(comments)) return this.setVisiable(false);
    if (getRepeatSourceComment(comments)) return this.setVisiable(false);
  }

  click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    let comments = KoobooComment.getComments(element);
    let comment = getEditableComment(comments)!;
    let aroundScopeComment = KoobooComment.getAroundScopeComments(element);
    let el = getUnpollutedEl(element)!;
    let oldValue = element.parentElement!.innerHTML;
    let guid = setGuid(element.parentElement!);
    if (aroundScopeComment) {
      let { nodes } = getWrapDom(element, aroundScopeComment.source);
      for (const node of nodes) {
        if (node instanceof HTMLElement) markDirty(node, true);
      }
    } else {
      markDirty(element.parentElement!);
    }

    let aroundComments = KoobooComment.getAroundComments(element);
    if (aroundComments.length > 0) {
      let { nodes } = getWrapDom(element, aroundComments[aroundComments.length - 1].uid);
      nodes.forEach(f => f.parentElement!.removeChild(f));
    } else {
      element.parentElement!.removeChild(element);
    }

    var log = [];
    if (aroundScopeComment && koobooId) {
      log.push(...aroundScopeComment.infos);
      log.push(kvInfo.delete);
      log.push(kvInfo.koobooId(koobooId));
    } else {
      if (el == element) {
        log.push(...comments.find(f => f.scope)!.infos);
        log.push(kvInfo.delete);
        log.push(kvInfo.koobooId(koobooId));
      } else {
        log.push(...comment.infos);
        log.push(kvInfo.value(clearKoobooInfo(el.innerHTML)), kvInfo.koobooId(el.getAttribute(KOOBOO_ID)));
      }
    }
    let operation = new operationRecord([new InnerHtmlUnit(oldValue)], [new Log(log)], guid);
    context.operationManager.add(operation);
    emitHoverEvent(document.body);
    emitSelectedEvent();
  }
}
