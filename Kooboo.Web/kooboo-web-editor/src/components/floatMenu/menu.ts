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

export function createMenu() {
  const { container, setExpandBtnVisiable, updatePosition } = createContainer();
  const items = [
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
    createDeleteHtmlBlockItem()
  ] as MenuItem[];

  for (const i of items) {
    container.appendChild(i.el);
  }

  const update = (x: number, y: number) => {
    let pageHeight = getMaxHeight();
    let pagewidth = getMaxWidth();
    container.style.display = "block";
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let elements = getParentElements(args.element);
    let canExpand = elements.findIndex(f => f instanceof HTMLBodyElement) != 0;
    setExpandBtnVisiable(canExpand);
    for (const i of items) {
      i.update(comments);
    }

    updatePosition(x, y, pageHeight, pagewidth);
  };

  const hidden = () => {
    container.style.display = "none";
  };

  return {
    container,
    update,
    hidden
  };
}
