import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getAttributeComment } from "../utils";
import { setGuid } from "@/kooboo/utils";
import { operationRecord } from "@/operation/Record";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { KoobooComment } from "@/kooboo/KoobooComment";
import BaseMenuItem from "./BaseMenuItem";
import { Menu } from "../menu";
import { kvInfo } from "@/common/kvInfo";

export default class EditRepeatImageItem extends BaseMenuItem {
  constructor(parentMenu: Menu) {
    super(parentMenu);

    const { el, setVisiable } = this.createItem(TEXT.EDIT_IMAGE);
    this.el = el;
    this.el.addEventListener("click", this.click.bind(this));
    this.setVisiable = setVisiable;
  }

  el: HTMLElement;

  setVisiable: (visiable: boolean) => void;

  update(): void {
    this.setVisiable(true);
    let { element } = context.lastSelectedDomEventArgs;
    let aroundComments = KoobooComment.getAroundComments(element);
    if (!isImg(element)) return this.setVisiable(false);
    if (!aroundComments.find(f => f.getValue("attribute") == "src")) return this.setVisiable(false);
  }

  click() {
    let { element } = context.lastSelectedDomEventArgs;
    this.parentMenu.hidden();

    let comments = KoobooComment.getAroundComments(element);
    let comment = comments.find(f => f.getValue("attribute") == "src")!;
    let img = element as HTMLImageElement;
    let startContent = img.getAttribute("src")!;
    pickImg(path => {
      img.src = path;
      let guid = setGuid(img);
      let value = img.getAttribute("src")!;
      let unit = new AttributeUnit(startContent, "src");
      let log = [...comment.infos, kvInfo.value(value)];
      let record = new operationRecord([unit], log, guid);
      context.operationManager.add(record);
    });
  }
}
