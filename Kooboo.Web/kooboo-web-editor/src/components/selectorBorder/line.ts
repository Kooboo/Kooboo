import { HOVER_BORDER_WIDTH, STANDARD_Z_INDEX } from "../../common/constants";
import { Position, updatePositon } from "@/common/Position";

const applyStyle = (css: CSSStyleDeclaration) => {
  css.position = "absolute";
  css.display = "block";
  css.pointerEvents = "none";
  css.backgroundColor = "#1fb5f6a3";
  css.width = HOVER_BORDER_WIDTH + "px";
  css.height = HOVER_BORDER_WIDTH + "px";
  css.zIndex = STANDARD_Z_INDEX - 2 + "";
};

export const createLine = () => {
  let el = document.createElement("div");
  applyStyle(el.style);

  const update = (position: Position) => updatePositon(position, el);

  return { el, update };
};
