import { createBlock } from "./block";
import { getMaxHeight } from "@/dom/utils";

export const createShade = () => {
  const blocks = [createBlock(), createBlock(), createBlock(), createBlock()];
  const el = document.createElement("div");
  blocks.forEach(i => el.appendChild(i.el));

  const updatePosition = (referenceEl: HTMLElement) => {
    el.style.display = "block";
    let [top, right, bottom, left] = blocks;
    let referenceRect = referenceEl.getBoundingClientRect();
    let bodyRect = document.body.getBoundingClientRect();

    top.update({
      top: 0,
      left: 0,
      right: 0,
      height: referenceRect.top - bodyRect.top
    });

    right.update({
      top: referenceRect.top - bodyRect.top,
      left: referenceRect.left + referenceRect.width,
      right: 0,
      height: referenceRect.height
    });

    bottom.update({
      top: referenceRect.top - bodyRect.top + referenceRect.height,
      left: 0,
      right: 0,
      height: getMaxHeight() - (referenceRect.top + bodyRect.top + referenceRect.height)
    });

    left.update({
      top: referenceRect.top - bodyRect.top,
      left: 0,
      width: referenceRect.left,
      height: referenceRect.height
    });
  };

  const hidden = () => {
    el.style.display = "none";
  };

  return {
    el,
    updatePosition,
    hidden
  };
};
