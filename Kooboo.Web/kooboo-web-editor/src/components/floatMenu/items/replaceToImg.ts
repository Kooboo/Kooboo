import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg, isInTable } from "@/dom/utils";
import { getViewComment, getRepeatComment } from "../utils";
import { isDynamicContent, setGuid, markDirty, clearKoobooInfo } from "@/kooboo/utils";
import { createImagePicker } from "@/components/imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createImg } from "@/dom/element";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";

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
    let args = context.lastSelectedDomEventArgs;
    if (getRepeatComment(comments)) return this.setVisiable(false);
    if (getRelatedRepeatComment(args.element)) return this.setVisiable(false);
    if (!getViewComment(comments)) return this.setVisiable(false);
    if (isInTable(args.element)) return this.setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent && !koobooId) return this.setVisiable(false);
    if (isImg(args.element)) return this.setVisiable(false);
    if (parent && isDynamicContent(parent)) return this.setVisiable(false);
  }

  async click() {
    let args = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    setGuid(parent!);
    let startContent = parent!.innerHTML;
    try {
      let style = getComputedStyle(args.element);
      let width = style.width;
      let widthImportant = args.element.style.getPropertyPriority("width");
      let height = style.height;
      let heightImportant = args.element.style.getPropertyPriority("height");
      let display = style.display;
      let img = createImg();
      img.setAttribute(KOOBOO_ID, args.koobooId!);
      args.element.parentElement!.replaceChild(img, args.element);
      img.style.setProperty("width", width, widthImportant);
      img.style.setProperty("height", height, heightImportant);
      img.style.display = display;
      await createImagePicker(img);
      markDirty(parent!);
      let guid = setGuid(parent!);
      let value = clearKoobooInfo(parent!.innerHTML);
      let comment = getViewComment(comments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = DomLog.createUpdate(comment.nameorid!, value, koobooId!, comment.objecttype!);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    } catch (error) {
      parent!.innerHTML = startContent;
    }
  }
}
