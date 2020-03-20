import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, markDirty, getUnpollutedEl, isDynamicContent, getWrapDom, isDirty, getWarpContent } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { getEditableComment, getRepeatSourceComment, ElementAnalyze } from "../utils";
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

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let { operability, comments, kooobooIdEl, fieldComment } = ElementAnalyze(element);
    if (!operability || !comments) return this.setVisiable(false);
    if (!kooobooIdEl && !fieldComment) return this.setVisiable(false);
    if (getRepeatSourceComment(comments)) return this.setVisiable(false);
  }

  click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    let { kooobooIdEl, fieldComment, scopeComment } = ElementAnalyze(element);
    this.parentMenu.hidden();
    var log = [];
    let oldValue = element.parentElement!.innerHTML;
    let guid = setGuid(element.parentElement!);
    let aroundScopeComment = KoobooComment.getAroundScopeComments(element);

    if (aroundScopeComment) {
      let { nodes } = getWrapDom(element, aroundScopeComment.source);
      for (const node of nodes) {
        if (node instanceof HTMLElement) markDirty(node, true);
      }
    } else {
      markDirty(element.parentElement!);
    }

    let placeholder = new Text("");
    let aroundComments = KoobooComment.getAroundComments(element);
    if (aroundComments.length > 0) {
      let { nodes } = getWrapDom(element, aroundComments[aroundComments.length - 1].uid);
      nodes[0].parentNode!.insertBefore(placeholder, nodes[0]);
      nodes.forEach(f => f.parentElement!.removeChild(f));
    } else {
      element.parentElement!.insertBefore(placeholder, element);
      element.parentElement!.removeChild(element);
    }

    if (element == kooobooIdEl) {
      log.push(...scopeComment!.infos, kvInfo.delete, kvInfo.koobooId(koobooId));
    } else {
      let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(placeholder!);
      let comment = fieldComment ? fieldComment : scopeComment;
      koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
      log.push(...comment!.infos, kvInfo.value(clearKoobooInfo(content)), kvInfo.koobooId(koobooId));
    }

    let operation = new operationRecord([new InnerHtmlUnit(oldValue)], [new Log(log)], guid);
    context.operationManager.add(operation);
    emitHoverEvent(document.body);
    emitSelectedEvent();
  }
}
