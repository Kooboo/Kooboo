import { createLine } from "./line";
import { createDiv } from "@/dom/element";

export const createRect = (width: number, color: string) => {
  const lines = [createLine(width, color), createLine(width, color), createLine(width, color), createLine(width, color)];
  const el = createDiv();
  lines.forEach(i => el.appendChild(i.el));

  const updatePosition = (referenceEl: HTMLElement) => {
    el.style.display = "block";

    let [top, right, bottom, left] = lines;

    let referenceRect = referenceEl.getBoundingClientRect();
    let htmlRect = document.documentElement.getBoundingClientRect();

    top.update({
      top: referenceRect.top - htmlRect.top,

      left: referenceRect.left - htmlRect.left + width,

      width: referenceEl.offsetWidth - width * 2
    });

    right.update({
      top: referenceRect.top - htmlRect.top,

      left: referenceRect.left - htmlRect.left - width + referenceEl.offsetWidth,

      height: referenceEl.offsetHeight
    });

    bottom.update({
      top: referenceRect.top - htmlRect.top + referenceEl.offsetHeight - width,
      left: referenceRect.left - htmlRect.left + width,
      width: referenceEl.offsetWidth - width * 2
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
