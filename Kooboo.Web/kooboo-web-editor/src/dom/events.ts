import context from "@/common/context";
import { getCloseElement } from "@/kooboo/utils";
import { HoverDomEventArgs } from "@/events/HoverDomEvent";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";

export function hover(e: MouseEvent) {
  e.stopPropagation();
  let args = context.lastHoverDomEventArgs;
  if (context.editing) return;
  let el = e.target as HTMLElement;
  if (args && args.element == el) return;
  let closeElement = getCloseElement(el);
  if (!closeElement) return;
  args = new HoverDomEventArgs(el, closeElement);
  context.lastMouseEventArg = e;
  context.hoverDomEvent.emit(args);
}

export function emitHoverEvent(el: HTMLElement) {
  let closeElement = getCloseElement(el);
  if (closeElement == null) return;
  context.hoverDomEvent.emit(new HoverDomEventArgs(el, closeElement));
}

export function emitSelectedEvent() {
  let element = context.lastHoverDomEventArgs.closeElement;
  var args = new SelectedDomEventArgs(element);
  context.domChangeEvent.emit(args);
}
