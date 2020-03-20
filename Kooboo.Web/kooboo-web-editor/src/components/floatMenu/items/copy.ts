import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, getWrapDom, getWarpContent } from "@/kooboo/utils";
import { TEXT } from "@/common/lang";
import { getRepeatSourceComment, ElementAnalyze } from "../utils";
import { operationRecord } from "@/operation/Record";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { kvInfo } from "@/common/kvInfo";
import { KOOBOO_ID } from "@/common/constants";
import { Log } from "@/operation/Log";

export default class CopyItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.COPY);
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
    let { element } = context.lastSelectedDomEventArgs;
    let { unpollutedEl, koobooId, kooobooIdEl, scopeComment, fieldComment } = ElementAnalyze(element);
    this.parentMenu.hidden();
    var log = [];
    let cloneElement = element.cloneNode(true) as HTMLElement;
    let guid = setGuid(element.parentElement!);
    let oldValue = element.parentElement!.innerHTML;
    let aroundComments = KoobooComment.getAroundComments(element!);

    if (aroundComments.length > 0) {
      let { nodes, endNode } = getWrapDom(element!, aroundComments[aroundComments.length - 1].uid);
      for (const node of nodes.reverse()) {
        let cloned = node.cloneNode(true);
        if (KoobooComment.isComment(cloned)) {
          let koobooComment = new KoobooComment(cloned);
          koobooComment.setValue("uid", koobooComment.uid + "_copy");
          cloned = koobooComment.ToComment();
        }
        element.parentElement!.insertBefore(cloned, endNode!.nextSibling);
      }
    } else {
      element.parentElement!.insertBefore(cloneElement, element.nextSibling);
    }

    if (element == kooobooIdEl) {
      log.push(...scopeComment!.infos, kvInfo.copy, kvInfo.koobooId(koobooId));
    } else {
      let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(unpollutedEl!);
      let comment = fieldComment ? fieldComment : scopeComment;
      koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
      log.push(...comment!.infos, kvInfo.value(clearKoobooInfo(content)), kvInfo.koobooId(koobooId));
    }

    let aroundScopeComment = KoobooComment.getAroundScopeComments(element!);
    if (aroundScopeComment) {
      let { nodes } = getWrapDom(element, aroundScopeComment.source);
      for (const node of nodes) {
        if (node instanceof HTMLElement) markDirty(node, true);
      }
    } else {
      markDirty(element.parentElement!);
    }

    let operation = new operationRecord([new InnerHtmlUnit(oldValue)], [new Log(log)], guid);
    context.operationManager.add(operation);
  }
}
