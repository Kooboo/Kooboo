import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, getUnpollutedEl, isDynamicContent, getWrapDom, isDirty } from "@/kooboo/utils";
import { TEXT } from "@/common/lang";
import { getRepeatSourceComment, getEditableComment } from "../utils";
import { isBody } from "@/dom/utils";
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

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    if (isBody(element)) return this.setVisiable(false);
    let el = getUnpollutedEl(element);
    if (!el && (!KoobooComment.getAroundScopeComments(element) || !koobooId || isDirty(element))) return this.setVisiable(false);
    if (!getEditableComment(comments)) return this.setVisiable(false);
    if (getRepeatSourceComment(comments)) return this.setVisiable(false);
    if (el && isDynamicContent(el)) return this.setVisiable(false);
  }

  click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    let comments = KoobooComment.getComments(element);
    let comment = getEditableComment(comments)!;
    let el = getUnpollutedEl(element)!;
    let aroundScopeComment = KoobooComment.getAroundScopeComments(element);
    let aroundComments = KoobooComment.getAroundComments(element);
    let cloneElement = element.cloneNode(true) as HTMLElement;
    let guid = setGuid(element.parentElement!);
    let oldValue = element.parentElement!.innerHTML;

    if (aroundComments.length > 0) {
      let { nodes, endNode } = getWrapDom(element, aroundComments[aroundComments.length - 1].uid);
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

    if (aroundScopeComment) {
      let { nodes } = getWrapDom(element, aroundScopeComment.source);
      for (const node of nodes) {
        if (node instanceof HTMLElement) markDirty(node, true);
      }
    } else {
      markDirty(element.parentElement!);
    }

    var log = [];
    if (aroundScopeComment) {
      log.push(...aroundScopeComment.infos);
      log.push(kvInfo.copy);
      log.push(kvInfo.koobooId(koobooId));
    } else {
      if (el == element) {
        log.push(...comments.find(f => f.scope)!.infos);
        log.push(kvInfo.copy);
        log.push(kvInfo.koobooId(koobooId));
      } else {
        log.push(...comment.infos);
        log.push(kvInfo.value(clearKoobooInfo(el.innerHTML)), kvInfo.koobooId(el.getAttribute(KOOBOO_ID)));
      }
    }

    let operation = new operationRecord([new InnerHtmlUnit(oldValue)], [new Log(log)], guid);
    context.operationManager.add(operation);
  }
}
