import { isSkipHover } from "../../common/dom";
import context from "../../context";
import { getCloseElement } from "../../common/koobooInfo";
import { HoverDomEventArgs } from "../../events/HoverDomEvent";

function hover(e: MouseEvent) {
  e.stopPropagation();

  if (isSkipHover(e) || context.editing) return;

  let el = e.target as HTMLElement;

  if (
    context.lastHoverDomEventArgs &&
    context.lastHoverDomEventArgs.element == el
  ) {
    return;
  }

  let closeElement = getCloseElement(el);
  if (closeElement == null) return;
  var args = new HoverDomEventArgs(el, closeElement);
  context.lastMouseEventArg = e;
  context.hoverDomEvent.emit(args);
}

export function domHover(document: Document) {
  document.body.addEventListener("mouseover", hover);
  const mouseenter = (e: MouseEvent) => {
    hover(e);
    document.body.removeEventListener("mousemove", mouseenter);
  };
  document.body.addEventListener("mousemove", mouseenter);
}
