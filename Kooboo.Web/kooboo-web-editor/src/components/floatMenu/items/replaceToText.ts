import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg, isInTable, getParentElements } from "@/dom/utils";
import { getEditableComment } from "../utils";
import { setGuid, getUnpollutedEl, clearKoobooInfo, isDynamicContent } from "@/kooboo/utils";
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

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let parents = getParentElements(element);
    if (parents.find(f => f.tagName.toLowerCase() == "p")) return this.setVisiable(false);
    if (!isImg(element)) return this.setVisiable(false);
    if (isInTable(element)) return this.setVisiable(false);
    if (!getEditableComment(comments)) return this.setVisiable(false);
    let el = getUnpollutedEl(element);
    let parent = el == element ? element.parentElement! : el;
    if (!parent) return this.setVisiable(false);
    if (isDynamicContent(parent)) return this.setVisiable(false);
  }

  async click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    let el = getUnpollutedEl(element)!;
    let parent = el == element ? el.parentElement! : el;
    let comments = KoobooComment.getComments(parent!);
    let comment = getEditableComment(comments)!;
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
    text.setAttribute(KOOBOO_ID, koobooId!);
    text.setAttribute(KOOBOO_DIRTY, "");
    text.style.setProperty("width", width, widthImportant);
    text.style.setProperty("height", height, heightImportant);
    text.style.display = display!.startsWith("inline") ? "inline-block" : "block";
    emitHoverEvent(text);
    emitSelectedEvent();

    const onSave = () => {
      let unit = new InnerHtmlUnit(startContent);
      let log = new Log([...comment.infos, kvInfo.value(clearKoobooInfo(parent.innerHTML)), kvInfo.koobooId(parent.getAttribute(KOOBOO_ID))]);
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
