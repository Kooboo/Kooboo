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
import { getMaxHeight, getMaxWidth } from "@/dom/utils";
import { createClickItem } from "./items/click";

export function createMenu() {
  const container = createContainer();
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
    createClickItem()
  ] as MenuItem[];

  for (const i of items) {
    container.el.appendChild(i.el);
  }

  const update = (x: number, y: number) => {
    let pageHeight = getMaxHeight();
    let pagewidth = getMaxWidth();
    container.el.style.display = "block";

    for (const i of items) {
      i.update();
    }

    container.updatePosition(x, y, pageHeight, pagewidth);
  };

  const hidden = () => {
    container.el.style.display = "none";
  };

  return {
    el: container.el,
    update,
    hidden
  };
}
