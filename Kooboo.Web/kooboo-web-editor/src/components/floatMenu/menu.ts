import { createContainer, MenuItem } from "./basic";
import { createCopyItem } from "./items/copy";
import { createDeleteItem } from "./items/delete";
import { createEditItem } from "./items/edit";
import { createEditImageItem } from "./items/editImage";
import { createEditLinkItem } from "./items/editLink";
import { createJumpLinkItem } from "./items/jumpLink";
import { createEditHtmlBlockItem } from "./items/editHtmlBlock";
import { createEditRepeatItem } from "./items/editRepeat";
import { createEditMenuItem } from "./items/editMenu";
import { createEditStyleItem } from "./items/editStyle";
import { createCopyRepeatItem } from "./items/copyRepeat";
import { createDeleteRepeatItem } from "./items/deleteRepeat";

export function createMenu() {
  const container = createContainer();
  const items = [
    createEditItem(),
    createCopyItem(),
    createDeleteItem(),
    createEditImageItem(),
    createEditLinkItem(),
    createJumpLinkItem(),
    createEditHtmlBlockItem(),
    createEditRepeatItem(),
    createCopyRepeatItem(),
    createDeleteRepeatItem(),
    createEditMenuItem(),
    createEditStyleItem()
  ] as MenuItem[];

  for (const i of items) {
    container.el.appendChild(i.el);
  }

  const update = (x: number, y: number) => {
    container.el.style.display = "block";
    container.updatePosition(x, y);
    for (const i of items) {
      i.update();
    }
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
