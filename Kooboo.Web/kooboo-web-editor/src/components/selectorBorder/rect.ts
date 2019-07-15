import { createLine } from "./line";
import { HOVER_BORDER_WIDTH } from "../../common/constants";
import createDiv from "@/dom/div";

export const createRect = () => {
  const lines = [createLine(), createLine(), createLine(), createLine()];
  const el = createDiv();
  lines.forEach(i => el.appendChild(i.el));

  const updatePosition = (referenceEl: HTMLElement) => {
    el.style.display = "block";

    let [top, right, bottom, left] = lines;

    let referenceRect = referenceEl.getBoundingClientRect();
    let htmlRect = document.documentElement.getBoundingClientRect();

    top.update({
      top: referenceRect.top - htmlRect.top,

      left: referenceRect.left - htmlRect.left + HOVER_BORDER_WIDTH,

      width: referenceEl.offsetWidth - HOVER_BORDER_WIDTH * 2
    });

    right.update({
      top: referenceRect.top - htmlRect.top,

      left: referenceRect.left - htmlRect.left - HOVER_BORDER_WIDTH + referenceEl.offsetWidth,

      height: referenceEl.offsetHeight
    });

    bottom.update({
      top: referenceRect.top - htmlRect.top + referenceEl.offsetHeight - HOVER_BORDER_WIDTH,
      left: referenceRect.left - htmlRect.left + HOVER_BORDER_WIDTH,
      width: referenceEl.offsetWidth - HOVER_BORDER_WIDTH * 2
    });

    left.update({
      top: referenceRect.top - htmlRect.top,

      left: referenceRect.left - htmlRect.left,

      height: referenceEl.offsetHeight
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
