import context from "@/common/context";
import { setGuid, markDirty, clearKoobooInfo, getUnpollutedEl } from "@/kooboo/utils";
import { TEXT } from "@/common/lang";
import { getEditComment } from "../utils";
import { isBody } from "@/dom/utils";
import { operationRecord } from "@/operation/Record";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { kvInfo } from "@/common/kvInfo";

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
    if (!getEditComment(comments)) return this.setVisiable(false);
    if (!getUnpollutedEl(element, false)) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(element);
    let el = getUnpollutedEl(element);
    let cloneElement = element.cloneNode(true) as HTMLElement;
    let guid = setGuid(el!);
    let oldValue = el!.innerHTML;
    element.parentElement!.insertBefore(cloneElement, element.nextSibling);
    markDirty(el!);
    let value = clearKoobooInfo(el!.innerHTML);
    let unit = new InnerHtmlUnit(oldValue);
    let comment = getEditComment(comments)!;
    let log = [...comment.infos, kvInfo.value(value)];
    let operation = new operationRecord([unit], log, guid);
    context.operationManager.add(operation);
  }
}
