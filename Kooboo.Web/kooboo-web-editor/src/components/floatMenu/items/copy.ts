import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, getUnpollutedEl } from "@/kooboo/utils";
import { TEXT } from "@/common/lang";
import { getScopeComnent } from "../utils";
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
    let { element } = context.lastSelectedDomEventArgs;
    if (isBody(element)) return this.setVisiable(false);
    if (!getScopeComnent(comments)) return this.setVisiable(false);
    if (!getUnpollutedEl(element)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(element);
    let el = getUnpollutedEl(element)!;
    let parent = el == element ? el.parentElement! : el;
    let cloneElement = element.cloneNode(true) as HTMLElement;
    let guid = setGuid(el.parentElement!);
    let oldValue = el.parentElement!.innerHTML;
    element.parentElement!.insertBefore(cloneElement, element.nextSibling);
    markDirty(el.parentElement!);
    let value = clearKoobooInfo(parent!.innerHTML);
    let unit = new InnerHtmlUnit(oldValue);
    let comment = getScopeComnent(comments)!;
    let log = new Log([...comment.infos, kvInfo.value(value), kvInfo.koobooId(parent.getAttribute(KOOBOO_ID))];) 
    let operation = new operationRecord([unit], [log], guid);
    context.operationManager.add(operation);
  }
}
