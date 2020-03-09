import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg, isInTable } from "@/dom/utils";
import { setGuid, markDirty, clearKoobooInfo, getUnpollutedEl, isDynamicContent } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createImg } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { getEditableComment } from "../utils";
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

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    if (isImg(element)) return this.setVisiable(false);
    if (!getEditableComment(comments)) return this.setVisiable(false);
    if (isInTable(element)) return this.setVisiable(false);
    let el = getUnpollutedEl(element);
    let parent = el == element ? element.parentElement! : el;
    if (!parent) return this.setVisiable(false);
    if (isDynamicContent(parent)) return this.setVisiable(false);
  }

  async click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let el = getUnpollutedEl(element)!;
    let parent = el == element ? element.parentElement! : el;
    let comments = KoobooComment.getComments(parent);
    let comment = getEditableComment(comments)!;
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
      img.setAttribute(KOOBOO_ID, koobooId!);
      element.parentElement!.replaceChild(img, element);
      img.style.setProperty("width", width, widthImportant);
      img.style.setProperty("height", height, heightImportant);
      img.style.display = display;
      await createImagePicker(img);
      markDirty(parent!);
      let unit = new InnerHtmlUnit(startContent);
      let log = new Log([...comment.infos, kvInfo.value(clearKoobooInfo(parent.innerHTML)), kvInfo.koobooId(parent.getAttribute(KOOBOO_ID))]);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      parent!.innerHTML = startContent;
    }
  }
}
