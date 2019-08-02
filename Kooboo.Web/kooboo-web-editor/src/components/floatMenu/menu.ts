import { createContainer, MenuItem } from "./basic";
import { createCopyItem } from "./items/copy";
import { createDeleteItem } from "./items/delete";
import { createEditItem } from "./items/edit";
import { createEditImageItem } from "./items/editImage";
import { createEditLinkItem } from "./items/editLink";
import { createEnterLinkItem } from "./items/enterLink";
import { createEditHtmlBlockItem } from "./items/editHtmlBlock";
import { createEditRepeatItem } from "./items/editRepeat";
import { createEditMenuItem } from "./items/editMenu";
import { createEditStyleItem } from "./items/editStyle";
import { createCopyRepeatItem } from "./items/copyRepeat";
import { createDeleteRepeatItem } from "./items/deleteRepeat";
import { createReplaceToImgItem } from "./items/replaceToImg";
import { createReplaceToTextItem } from "./items/replaceToText";
import { createEditRepeatImageItem } from "./items/editRepeatImage";
import { createEditRepeatLinkItem } from "./items/editRepeatLink";
import { getMaxHeight, getMaxWidth, getParentElements } from "@/dom/utils";
import { createClickItem } from "./items/click";
import { createDeleteHtmlBlockItem } from "./items/deleteHtmlBlock";
import { KoobooComment } from "@/kooboo/KoobooComment";
import context from "@/common/context";
// import { createConvert } from "./items/convert";
import { createInlineEditHtmlBlockItem } from "./items/inlineEditHtmlBlock";

export function createMenu() {
  return new Menu();
}

class Menu {
  constructor(){
    const { container, setExpandBtnVisiable, updatePosition } = createContainer();

    for (const i of this.items) {
      container.appendChild(i.el);
    }

    this.container = container;
    this.setExpandBtnVisiable = setExpandBtnVisiable;
    this.updatePosition = updatePosition;
  }

  items = [
    createEditItem(),
    createCopyItem(),
    createDeleteItem(),
    createEditImageItem(),
    createEditLinkItem(),
    createEnterLinkItem(),
    createEditHtmlBlockItem(),
    createEditRepeatItem(),
    createCopyRepeatItem(),
    createDeleteRepeatItem(),
    createEditMenuItem(),
    createEditStyleItem(),
    createReplaceToImgItem(),
    createReplaceToTextItem(),
    createEditRepeatImageItem(),
    createEditRepeatLinkItem(),
    createClickItem(),
    createDeleteHtmlBlockItem(),
    // createConvert(),
    createInlineEditHtmlBlockItem()
  ] as MenuItem[];

  container:HTMLDivElement;

  setExpandBtnVisiable:(visiable: boolean)=>void;

  updatePosition:(x: number, y: number, pageHeight:number, pageWidth:number)=>void;

  update = (x: number, y: number) => {
    let pageHeight = getMaxHeight();
    let pagewidth = getMaxWidth();
    this.container.style.display = "block";
    this.container.style.overflow = "visible";
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let elements = getParentElements(args.element);
    let canExpand = elements.findIndex(f => f instanceof HTMLBodyElement) != 0;
    this.setExpandBtnVisiable(canExpand);
    for (const i of this.items) {
      i.update(comments);
    }

    this.updatePosition(x, y, pageHeight, pagewidth);
  };

  hidden = () => {
    this.container.style.display = "none";
  };
}