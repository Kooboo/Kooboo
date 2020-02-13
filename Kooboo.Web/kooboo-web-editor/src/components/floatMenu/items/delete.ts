import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { setGuid, clearKoobooInfo, markDirty, getUnpollutedEl } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { getScopeComnent } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { KOOBOO_ID } from "@/common/constants";
import { kvInfo } from "@/common/kvInfo";

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
    if (!el || !el.parentElement) return this.setVisiable(false);
    if (!getScopeComnent(comments)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    let el = getUnpollutedEl(element)!;
    let comments = KoobooComment.getComments(element);
    let comment = getScopeComnent(comments)!;
    let parent = el == element ? element.parentElement! : el;
    let oldValue = parent.innerHTML;
    let guid = setGuid(parent);
    element.parentElement!.removeChild(element);
    markDirty(parent);
    var log = [...comment.infos];
    if (el == element) {
      log.push(kvInfo.koobooId(parent.getAttribute(KOOBOO_ID)));
      log.push(kvInfo.delete);
    } else {
      log.push(kvInfo.value(clearKoobooInfo(parent.innerHTML)), kvInfo.koobooId(parent.getAttribute(KOOBOO_ID)));
    }
    let operation = new operationRecord([new InnerHtmlUnit(oldValue)], log, guid);
    context.operationManager.add(operation);
  }
}
