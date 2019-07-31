import { createBlock } from "./block";
import { getMaxHeight } from "@/dom/utils";
import { createDiv } from "@/dom/element";
import closeIcon from "@/assets/icons/guanbi.svg";
import { STANDARD_Z_INDEX } from "@/common/constants";
import context from "@/common/context";

export const createShade = () => {
  const blocks = [createBlock(), createBlock(), createBlock(), createBlock()];
  const el = createDiv();
  blocks.forEach(i => el.appendChild(i.el));
  el.appendChild(createCloseBtn());

  const updatePosition = (referenceEl: HTMLElement) => {
    el.style.display = "block";
    let [top, right, bottom, left] = blocks;
    let referenceRect = referenceEl.getBoundingClientRect();
    let htmlRect = document.documentElement.getBoundingClientRect();

    top.update({
      top: 0,
      left: 0,
      right: 0,
      height: referenceRect.top - htmlRect.top
    });

    right.update({
      top: referenceRect.top - htmlRect.top,
      left: referenceRect.left + referenceRect.width,
      right: 0,
      height: referenceRect.height
    });

    bottom.update({
      top: referenceRect.top - htmlRect.top + referenceRect.height,
      left: 0,
      right: 0,
      height: getMaxHeight() - (referenceRect.top + htmlRect.top + referenceRect.height)
    });

    left.update({
      top: referenceRect.top - htmlRect.top,
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

function createCloseBtn() {
  let btn = createDiv();
  btn.classList.add("kb_web_editor_shade_close_btn");
  btn.style.backgroundImage = `url(${closeIcon})`;
  btn.style.zIndex = STANDARD_Z_INDEX + "";
  btn.onclick = () => context.closeEditingEvent.emit();
  return btn;
}
