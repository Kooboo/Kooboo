import { createButton } from "./button";
import moveIcon from "@/assets/icons/drag-move--fill.svg";
import { TEXT } from "@/common/lang";

export function createMoveButton(container: HTMLElement) {
  var btn = createButton(moveIcon, TEXT.MOVE);
  btn.draggable = true;
  btn.style.cursor = "move";

  btn.ondrag = e => {
    if (e.x == 0 || e.y == 0) return;
    container.style.top = e.y - 25 + "px";
    container.style.left = e.x - 25 + "px";
  };

  return btn;
}
