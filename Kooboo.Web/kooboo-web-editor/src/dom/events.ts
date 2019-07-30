import { isInEditorContainer } from "./utils";
import context from "@/common/context";
import { getCloseElement } from "@/kooboo/utils";
import { HoverDomEventArgs } from "@/events/HoverDomEvent";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";

function hover(e: MouseEvent) {
  e.stopPropagation();
  let args = context.lastHoverDomEventArgs;
  if (isInEditorContainer(e) || context.editing) return;
  let el = e.target as HTMLElement;
  if (args && args.element == el) return;
  let closeElement = getCloseElement(el);
  if (closeElement == null) return;
  args = new HoverDomEventArgs(el, closeElement);
  context.lastMouseEventArg = e;
  context.hoverDomEvent.emit(args);
}

export function listenHover() {
  document.body.addEventListener("mouseover", hover);
  const mouseenter = (e: MouseEvent) => {
    hover(e);
    document.body.removeEventListener("mousemove", mouseenter);
  };
  document.body.addEventListener("mousemove", mouseenter);
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