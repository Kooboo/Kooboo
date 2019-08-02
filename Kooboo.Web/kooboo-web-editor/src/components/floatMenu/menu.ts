import { createContainer } from "./basic";
import CopyItem from "./items/copy";
import DeleteItem from "./items/delete";
import EditItem from "./items/edit";
import EditImageItem from "./items/editImage";
import EditLinkItem from "./items/editLink";
import EnterLinkItem from "./items/enterLink";
import EditHtmlBlockItem from "./items/editHtmlBlock";
import EditRepeatItem from "./items/editRepeat";
import EditMenuItem from "./items/editMenu";
import EditStyleItem from "./items/editStyle";
import CopyRepeatItem from "./items/copyRepeat";
import DeleteRepeatItem from "./items/deleteRepeat";
import ReplaceToImgItem from "./items/replaceToImg";
import ReplaceToTextItem from "./items/replaceToText";
import EditRepeatImageItem from "./items/editRepeatImage";
import EditRepeatLinkItem from "./items/editRepeatLink";
import { getMaxHeight, getMaxWidth, getParentElements } from "@/dom/utils";
import ClickItem from "./items/click";
import DeleteHtmlBlockItem from "./items/deleteHtmlBlock";
import { KoobooComment } from "@/kooboo/KoobooComment";
import context from "@/common/context";
// import { createConvert } from "./items/convert";
import InlineEditHtmlBlockItem from "./items/inlineEditHtmlBlock";
import BaseMenuItem from "./items/BaseMenuItem";

export function createMenu() {
  let menu = new Menu();

  menu.addItem(new EditItem(menu));
  menu.addItem(new CopyItem(menu));
  menu.addItem(new DeleteItem(menu));
  menu.addItem(new EditImageItem(menu));
  menu.addItem(new EditLinkItem(menu));
  menu.addItem(new EnterLinkItem(menu));
  menu.addItem(new EditHtmlBlockItem(menu));
  menu.addItem(new EditRepeatItem(menu));
  menu.addItem(new CopyRepeatItem(menu));
  menu.addItem(new DeleteRepeatItem(menu));
  menu.addItem(new EditMenuItem(menu));
  menu.addItem(new EditStyleItem(menu));
  menu.addItem(new ReplaceToImgItem(menu));
  menu.addItem(new ReplaceToTextItem(menu));
  menu.addItem(new EditRepeatImageItem(menu));
  menu.addItem(new EditRepeatLinkItem(menu));
  menu.addItem(new ClickItem(menu));
  menu.addItem(new DeleteHtmlBlockItem(menu));
  menu.addItem(new InlineEditHtmlBlockItem(menu));

  return menu;
}

export class Menu {
  constructor() {
    const { container, setExpandBtnVisiable, updatePosition } = createContainer();

    for (const i of this.items) {
      container.appendChild(i.el);
    }

    this.container = container;
    this.setExpandBtnVisiable = setExpandBtnVisiable;
    this.updatePosition = updatePosition;
  }

  addItem(item: BaseMenuItem) {
    this.container.appendChild(item.el);
    this.items.push(item);
  }

  protected items = [] as BaseMenuItem[];

  container: HTMLDivElement;

  setExpandBtnVisiable: (visiable: boolean) => void;

  updatePosition: (x: number, y: number, pageHeight: number, pageWidth: number) => void;

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
