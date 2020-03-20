import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg, isInTable } from "@/dom/utils";
import { setGuid, markDirty, clearKoobooInfo, getWrapDom, getWarpContent } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createImg } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { ElementAnalyze } from "../utils";
import { Log } from "@/operation/Log";
import { kvInfo } from "@/common/kvInfo";

export default class ReplaceToImgItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.REPLACE_TO_IMG);
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
    if (kooobooIdEl == element) {
      var parent = ElementAnalyze(element.parentElement!);
      if (!parent.operability || !parent.kooobooIdEl) return this.setVisiable(false);
    }
    if (!kooobooIdEl && !fieldComment) return this.setVisiable(false);
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
    this.parentMenu.hidden();
    var parent = element.parentElement!;
    let guid = setGuid(parent);
    let startContent = parent.innerHTML;

    try {
      let style = getComputedStyle(element);
      let width = style.width;
      let widthImportant = element.style.getPropertyPriority("width");
      let height = style.height;
      let heightImportant = element.style.getPropertyPriority("height");
      let display = style.display;
      let img = createImg();
      img.setAttribute(KOOBOO_ID, element.getAttribute(KOOBOO_ID)!);
      element.parentElement!.replaceChild(img, element);
      img.style.setProperty("width", width, widthImportant);
      img.style.setProperty("height", height, heightImportant);
      img.style.display = display;
      await createImagePicker(img);
      let aroundScopeComment = KoobooComment.getAroundScopeComments(element!);

      if (aroundScopeComment) {
        let { nodes } = getWrapDom(img, aroundScopeComment.source);
        for (const node of nodes) {
          if (node instanceof HTMLElement) markDirty(node, true);
        }
      } else {
        markDirty(parent!);
      }

      let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(img!);
      let comment = fieldComment ? fieldComment : scopeComment;
      koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
      let log = new Log([...comment!.infos, kvInfo.value(clearKoobooInfo(content)), kvInfo.koobooId(koobooId)]);
      let unit = new InnerHtmlUnit(startContent);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      parent.innerHTML = startContent;
    }
  }
}
