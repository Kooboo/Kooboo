import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { createStyleEditor } from "@/components/styleEditor";
import { setGuid, clearKoobooInfo, getUnpollutedEl, getWarpContent, isDynamicContent } from "@/kooboo/utils";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { operationRecord } from "@/operation/Record";
import { getEditableComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { isImg, isBody } from "@/dom/utils";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { Log } from "@/operation/Log";
import { kvInfo } from "@/common/kvInfo";
import { KOOBOO_ID } from "@/common/constants";

export default class EditStyleItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT_STYLE);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let el = getUnpollutedEl(element);
    if (!el || isBody(el)) return this.setVisiable(false);
    if (el && isDynamicContent(el)) return this.setVisiable(false);
    if (isImg(element)) return this.setVisiable(false);
    if (!getEditableComment(comments)) return this.setVisiable(false);
    if (!getUnpollutedEl(element)) return this.setVisiable(false);
  }

  async click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(element);
    let comment = getEditableComment(comments)!;
    let el = getUnpollutedEl(element)!;
    const startContent = element.getAttribute("style");
    try {
      let logs = await createStyleEditor(element);
      let guid = setGuid(element);
      let unit = new AttributeUnit(startContent!, "style");
      if (logs.length == 0) return;

      let koobooId = el.getAttribute(KOOBOO_ID);
      let content = el.innerHTML;
      if (!koobooId) content = getWarpContent(el);
      if (element != el || !koobooId) {
        let infos = [...comment.infos, kvInfo.value(clearKoobooInfo(content)), kvInfo.koobooId(koobooId)];
        logs = [new Log(infos)];
      }

      let record = new operationRecord([unit], logs, guid);
      context.operationManager.add(record);
    } catch (error) {
      element.setAttribute("style", startContent!);
    }
  }
}
