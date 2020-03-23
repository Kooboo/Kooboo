import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { clearKoobooInfo, markDirty, setGuid, getUnpollutedEl, isDynamicContent, getWarpContent } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { setInlineEditor } from "@/components/richEditor";
import { clearContent, getEditableComment, isEditable, ElementAnalyze } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { kvInfo } from "@/common/kvInfo";
import { KOOBOO_ID } from "@/common/constants";
import { Log } from "@/operation/Log";

export default class EditItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT);
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
    if (!operability || !comments || !isEditable(element)) return this.setVisiable(false);
    if (!kooobooIdEl && !fieldComment) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    let { kooobooIdEl, fieldComment, koobooId, scopeComment } = ElementAnalyze(element);
    this.parentMenu.hidden();
    let startContent = element.innerHTML;
    const onSave = () => {
      if (clearContent(startContent) == clearContent(element.innerHTML)) return;
      if (kooobooIdEl) markDirty(kooobooIdEl);
      let guid = setGuid(element);
      let units = [new InnerHtmlUnit(startContent)];
      koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
      let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(element!);
      let comment = fieldComment ? fieldComment : scopeComment;
      let log = [...comment!.infos, kvInfo.koobooId(koobooId), kvInfo.value(clearKoobooInfo(content))];
      let operation = new operationRecord(units, [new Log(log)], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      element.innerHTML = startContent;
    };

    setInlineEditor({ selector: element, onSave, onCancel });
  }
}
