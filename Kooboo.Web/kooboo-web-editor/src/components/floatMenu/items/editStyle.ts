import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { createStyleEditor } from "@/components/styleEditor";
import { setGuid, clearKoobooInfo, getWarpContent } from "@/kooboo/utils";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { operationRecord } from "@/operation/Record";
import { ElementAnalyze } from "../utils";
import { isImg } from "@/dom/utils";
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

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let { operability, kooobooIdEl, fieldComment } = ElementAnalyze(element);
    if (isImg(element) || !operability) return this.setVisiable(false);
    if (!kooobooIdEl && !fieldComment) return this.setVisiable(false);
  }

  async click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();
    let { kooobooIdEl, fieldComment, koobooId, scopeComment } = ElementAnalyze(element);

    const startContent = element.getAttribute("style");
    try {
      let logs = await createStyleEditor(element);
      let guid = setGuid(element);
      let unit = new AttributeUnit(startContent!, "style");
      if (logs.length == 0) return;

      if (element != kooobooIdEl || (!kooobooIdEl && fieldComment)) {
        koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
        let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(element!);
        let comment = fieldComment ? fieldComment : scopeComment;
        let infos = [...comment!.infos, kvInfo.value(clearKoobooInfo(content)), kvInfo.koobooId(koobooId)];
        logs = [new Log(infos)];
      }

      let record = new operationRecord([unit], logs, guid);
      context.operationManager.add(record);
    } catch (error) {
      element.setAttribute("style", startContent!);
    }
  }
}
