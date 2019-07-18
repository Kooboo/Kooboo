import { STANDARD_Z_INDEX } from "../../common/constants";
import { Position, updatePositon } from "@/common/Position";
import { createDiv } from "@/dom/element";

export const createLine = (width: number, color: string) => {
  let el = createDiv();
  el.style.position = "absolute";
  el.style.display = "block";
  el.style.pointerEvents = "none";
  el.style.backgroundColor = color;
  el.style.width = width + "px";
  el.style.height = width + "px";
  el.style.zIndex = STANDARD_Z_INDEX - 2 + "";

  const update = (position: Position) => updatePositon(position, el);

  return { el, update };
};
