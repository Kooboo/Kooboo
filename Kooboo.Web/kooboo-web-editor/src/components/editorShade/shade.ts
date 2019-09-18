import { createBlock } from "./block";
import { createDiv } from "@/dom/element";
import closeIcon from "@/assets/icons/guanbi.svg";
import { STANDARD_Z_INDEX } from "@/common/constants";
import context from "@/common/context";

export const createShade = () => {
  const blocks = [createBlock(), createBlock(), createBlock(), createBlock()];
  const el = createDiv();
  blocks.forEach(i => el.appendChild(i.el));
  let closeBtn = createCloseBtn();
  el.appendChild(closeBtn);

  const updatePosition = (referenceEl: HTMLElement) => {
    el.style.display = "block";
    closeBtn.style.display = "block";
    let [top, right, bottom, left] = blocks;
    let referenceRect = referenceEl.getBoundingClientRect();
    top.update({
      top: 0,
      left: referenceRect.left,
      width: referenceRect.width,
      height: referenceRect.top < 0 ? 0 : referenceRect.top
    });

    right.update({
      top: 0,
      right: 0,
      left: referenceRect.left + referenceRect.width,
      bottom: 0
    });

    bottom.update({
      top: referenceRect.bottom < 0 ? 0 : referenceRect.bottom,
      left: referenceRect.left,
      width: referenceRect.width,
      bottom: 0
    });

    left.update({
      top: 0,
      left: 0,
      width: referenceRect.left,
      bottom: 0
    });
  };

  const hidden = () => {
    el.style.display = "none";
    closeBtn.style.display = "none";
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
