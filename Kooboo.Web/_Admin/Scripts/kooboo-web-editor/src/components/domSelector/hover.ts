import { isSkipHover } from "../../common/dom";
import context from "../../context";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";
import { getKoobooInfo, getCloseElement } from "../../common/koobooInfo";
import { HoverDomEventArgs } from "../../events/HoverDomEvent";

export function domHover(document: Document) {
  document.body.addEventListener("mouseover", e => {
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
    var args = new HoverDomEventArgs(el, closeElement);
    context.lastHoverDomEventArgs = args;
    context.hoverDomEvent.emit(args);
  });
}
