import { createContainer } from "./basic";
import { createCopyItem } from "./items/copy";
import { createDeleteItem } from "./items/delete";
import { createEditItem } from "./items/edit";
import { createEditImageItem } from "./items/editImage";
import { createEditLinkItem } from "./items/editLink";

export function createMenu() {
  const container = createContainer();
  const items = [
    createEditItem(),
    createCopyItem(),
    createDeleteItem(),
    createEditImageItem(),
    createEditLinkItem()
  ];

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
