import { createLine } from "./line";
import { HOVER_BORDER_WIDTH } from "../../common/constants";

export const createRect = () => {
  const lines = [createLine(), createLine(), createLine(), createLine()];
  const el = document.createElement("div");
  lines.forEach(i => el.appendChild(i.el));

  const updatePosition = (referenceEl: HTMLElement) => {
    el.style.display = "block";

    let [top, right, bottom, left] = lines;

    let referenceRect = referenceEl.getBoundingClientRect();
    let bodyRect = document.body.getBoundingClientRect();

    top.update({
      top: referenceRect.top - bodyRect.top,

      left: referenceRect.left - bodyRect.left + HOVER_BORDER_WIDTH,

      width: referenceEl.offsetWidth - HOVER_BORDER_WIDTH * 2
    });

    right.update({
      top: referenceRect.top - bodyRect.top,

      left: referenceRect.left - bodyRect.left - HOVER_BORDER_WIDTH + referenceEl.offsetWidth,

      height: referenceEl.offsetHeight
    });

    bottom.update({
      top: referenceRect.top - bodyRect.top + referenceEl.offsetHeight - HOVER_BORDER_WIDTH,
      left: referenceRect.left - bodyRect.left + HOVER_BORDER_WIDTH,
      width: referenceEl.offsetWidth - HOVER_BORDER_WIDTH * 2
    });

    left.update({
      top: referenceRect.top - bodyRect.top,

      left: referenceRect.left - bodyRect.left,

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
