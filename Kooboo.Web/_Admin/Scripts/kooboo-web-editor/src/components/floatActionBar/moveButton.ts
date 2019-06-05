import { createButton } from "./button";
import moveIcon from "../../assets/icons/drag-move--fill.svg";
import context from "../../context";

export function createMoveButton(document: Document, container: HTMLElement) {
  var btn = createButton(document, moveIcon);
  btn.draggable = true;
  btn.style.cursor = "move";

  btn.ondragstart = e => {
    context.lastSelectedDomEventArgs = undefined;
    context.editing = true;
  };

  btn.ondrag = e => {
    if (e.x == 0 || e.y == 0) return;
    container.style.top = e.y - 25 + "px";
    container.style.left = e.x - 25 + "px";
  };

  btn.ondragend = e => {
    context.editing = false;
  };

  return btn;
}
