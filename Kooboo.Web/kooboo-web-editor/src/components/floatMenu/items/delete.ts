import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, markDirty, getUnpollutedEl, isDynamicContent, getWrapDom } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { getScopeComnent } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { KOOBOO_ID } from "@/common/constants";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";

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
    let { element } = context.lastSelectedDomEventArgs;
    if (isBody(element)) return this.setVisiable(false);
    let el = getUnpollutedEl(element);
    if (!el && !KoobooComment.getAroundScopeComments(element)) return this.setVisiable(false);
    if (!getScopeComnent(comments)) return this.setVisiable(false);
    if (el && isDynamicContent(el)) return this.setVisiable(false);
  }

  click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    let comments = KoobooComment.getComments(element);
    let comment = getScopeComnent(comments)!;
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
    if (aroundScopeComment) {
      log.push(...aroundScopeComment.infos);
      log.push(kvInfo.delete);
      log.push(kvInfo.koobooId(koobooId));
    } else {
      log.push(...comment.infos);
      if (el == element) {
        log.push(kvInfo.delete);
        log.push(kvInfo.koobooId(koobooId));
      } else {
        log.push(kvInfo.value(clearKoobooInfo(el.innerHTML)), kvInfo.koobooId(el.getAttribute(KOOBOO_ID)));
      }
    }
    let operation = new operationRecord([new InnerHtmlUnit(oldValue)], [new Log(log)], guid);
    context.operationManager.add(operation);
  }
}
