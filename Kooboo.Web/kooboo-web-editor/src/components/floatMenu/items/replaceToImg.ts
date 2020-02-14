import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg, isInTable } from "@/dom/utils";
import { isDynamicContent, setGuid, markDirty, clearKoobooInfo, getUnpollutedEl } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createImg } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { getScopeComnent } from "../utils";
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
    if (!getScopeComnent(comments)) return this.setVisiable(false);
    if (isInTable(element)) return this.setVisiable(false);
    if (!getUnpollutedEl(element)) return this.setVisiable(false);
  }

  async click() {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(element);
    let el = getUnpollutedEl(element)!;
    let parent = el == element ? element.parentElement! : el;
    setGuid(el.parentElement!);
    let startContent = el.parentElement!.innerHTML;
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
      let guid = setGuid(element.parentElement!);
      let value = clearKoobooInfo(parent.innerHTML);
      let comment = getScopeComnent(comments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = new Log([...comment.infos, kvInfo.value(value), kvInfo.koobooId(parent.getAttribute(KOOBOO_ID))]);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      parent!.innerHTML = startContent;
    }
  }
}
