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
import { getMaxHeight, getMaxWidth, getParentElements, getScrollLeft, getScrollTop } from "@/dom/utils";
import ClickItem from "./items/click";
import { KoobooComment } from "@/kooboo/KoobooComment";
import context from "@/common/context";
import BaseMenuItem from "./items/BaseMenuItem";
import { createDiv } from "@/dom/element";
import { STANDARD_Z_INDEX } from "@/common/constants";
import { TEXT } from "@/common/lang";
import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";
import closeIcon from "@/assets/icons/guanbi.svg";
import expandIcon from "@/assets/icons/fangda.svg";
import EditColorItem from "./items/editColor";
import EditDataItem from "./items/editData";

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
  menu.addItem(new EditColorItem(menu));
  menu.addItem(new EditDataItem(menu));

  return menu;
}

export class Menu {
  constructor() {
    const { container, setExpandBtnVisiable, updatePosition } = this.createContainer();

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

  // 生成容器
  private createContainer() {
    let el = createDiv();
    el.classList.add("kb_web_editor_menu");
    el.style.zIndex = STANDARD_Z_INDEX + 1 + "";
    let { setExpandBtnVisiable, title } = this.createTitle();
    el.append(title);

    const updatePosition = (x: number, y: number, pageHeight: number, pageWidth: number) => {
      let rect = el.getBoundingClientRect();
      let scrollLeft = getScrollLeft();
      let scrollTop = getScrollTop();
      let limitWidth = window.innerWidth + scrollLeft;
      let limitHeight = window.innerHeight + scrollTop;
      if (x + rect.width > limitWidth) {
        x = x - rect.width + 3;
      }

      if (y + rect.height > limitHeight) {
        y = y - rect.height + 3;
        context.lastMouseEventArg = {
          isTrusted: true,
          pageX: x,
          pageY: y
        } as MouseEvent;
      }

      el.style.top = y + "px";
      el.style.left = x + "px";
    };

    return {
      container: el,
      updatePosition,
      setExpandBtnVisiable
    };
  }

  // 生成标题栏
  private createTitle() {
    const el = createDiv();
    el.classList.add("kb_web_editor_menu_title");
    el.appendChild(document.createTextNode(TEXT.MENU));
    el.appendChild(this.createCloseButton());
    let expandButton = this.createExpandButton();
    el.appendChild(expandButton);
    const setExpandBtnVisiable = (visiable: boolean) => {
      expandButton.style.display = visiable ? "block" : "none";
    };
    return { title: el, setExpandBtnVisiable };
  }

  // 生成关闭按钮
  private createCloseButton() {
    const el = createDiv();
    el.style.backgroundImage = `url(${closeIcon})`;
    el.onclick = () => this.hidden();
    return el;
  }

  // 生成扩大按钮
  private createExpandButton() {
    const el = createDiv();
    el.style.backgroundImage = `url(${expandIcon})`;
    el.title = TEXT.EXPAND_SELECTION;
    el.onclick = () => {
      if (context.editing) return;
      let el = context.lastHoverDomEventArgs!.closeElement;
      if (!el || !el.parentElement) return;
      emitHoverEvent(el.parentElement);
      emitSelectedEvent();
    };
    return el;
  }
}
