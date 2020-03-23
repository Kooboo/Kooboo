import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg, isInTable, getParentElements } from "@/dom/utils";
import { getEditableComment, ElementAnalyze } from "../utils";
import { setGuid, getUnpollutedEl, clearKoobooInfo, isDynamicContent, getWarpContent, getWrapDom, markDirty } from "@/kooboo/utils";
import { setInlineEditor } from "@/components/richEditor";
import { KOOBOO_ID, KOOBOO_DIRTY } from "@/common/constants";
import { emitSelectedEvent, emitHoverEvent } from "@/dom/events";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createP } from "@/dom/element";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { Log } from "@/operation/Log";
import { kvInfo } from "@/common/kvInfo";

export default class ReplaceToTextItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.REPLACE_TO_TEXT);
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
    if (!isImg(element) || !operability) return this.setVisiable(false);
    if (kooobooIdEl == element) {
      var parent = ElementAnalyze(element.parentElement!);
      if (!parent.operability || !parent.kooobooIdEl) return this.setVisiable(false);
    }
    if (!kooobooIdEl && !fieldComment) return this.setVisiable(false);
    if (isInTable(element)) return this.setVisiable(false);
    let parents = getParentElements(element);
    if (parents.find(f => f.tagName.toLowerCase() == "p")) return this.setVisiable(false);
    if (isInTable(element)) return this.setVisiable(false);
  }

  async click() {
    let { element } = context.lastSelectedDomEventArgs;
    let { scopeComment, kooobooIdEl, fieldComment, koobooId } = ElementAnalyze(element);
    if (kooobooIdEl == element) {
      var parentInfo = ElementAnalyze(element.parentElement!);
      kooobooIdEl = parentInfo.kooobooIdEl;
      koobooId == parentInfo.koobooId;
    }
    let parent = element.parentElement!;
    let startContent = parent.innerHTML;
    let text = createP();
    let style = getComputedStyle(element);
    let width = style.width;
    let widthImportant = element.style.getPropertyPriority("width");
    let height = style.height;
    let display = style.display;
    let heightImportant = element.style.getPropertyPriority("height");
    let guid = setGuid(parent);
    element.parentElement!.replaceChild(text, element);
    text.setAttribute(KOOBOO_ID, element.getAttribute(KOOBOO_ID)!);
    text.style.setProperty("width", width, widthImportant);
    text.style.setProperty("height", height, heightImportant);
    text.style.display = display!.startsWith("inline") ? "inline-block" : "block";
    emitHoverEvent(text);
    emitSelectedEvent();
    let aroundScopeComment = KoobooComment.getAroundScopeComments(text!);
    if (aroundScopeComment) {
      let { nodes } = getWrapDom(text, aroundScopeComment.source);
      for (const node of nodes) {
        if (node instanceof HTMLElement) markDirty(node, true);
      }
    } else {
      markDirty(parent!);
    }

    const onSave = () => {
      let unit = new InnerHtmlUnit(startContent);
      let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(text);
      let comment = fieldComment ? fieldComment : scopeComment;
      koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
      let log = new Log([...comment!.infos, kvInfo.value(clearKoobooInfo(content)), kvInfo.koobooId(koobooId)]);
      let operation = new operationRecord([unit], [log], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      parent.innerHTML = startContent;
    };

    await setInlineEditor({ selector: text, onSave, onCancel });
    this.parentMenu.hidden();
  }
}
