import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getAttributeComment } from "../utils";
import { setGuid } from "@/kooboo/utils";
import { operationRecord } from "@/operation/Record";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";

export function createEditRepeatImageItem(): MenuItem {
  return new EditRepeatImageItem();
}

class EditRepeatImageItem extends BaseMenuItem {
  constructor() {
    super();

    const { el, setVisiable } = createItem(TEXT.EDIT_IMAGE, MenuActions.editImage);
    this.el = el;
    this.el.addEventListener("click", this.click);
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(comments: KoobooComment[]): void {
    this.setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) return this.setVisiable(false);
    let comment = getAttributeComment(comments, "src");
    if (!comment || !comment.fieldname) return this.setVisiable(false);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let comment = getAttributeComment(comments, "src")!;
    let img = args.element as HTMLImageElement;
    let startContent = img.getAttribute("src")!;
    pickImg(path => {
      img.src = path;
      let guid = setGuid(img);
      let value = img.getAttribute("src")!;
      let unit = new AttributeUnit(startContent, "src");
      let log = ContentLog.createUpdate(comment.nameorid!, comment.fieldname!, value);
      let record = new operationRecord([unit], [log], guid);
      context.operationManager.add(record);
    });
  }
}
