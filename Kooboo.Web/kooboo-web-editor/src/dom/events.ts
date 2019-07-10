import { isInEditorContainer } from "./utils";
import context from "@/common/context";
import { getCloseElement } from "@/kooboo/utils";
import { HoverDomEventArgs } from "@/events/HoverDomEvent";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { KOOBOO_ID } from "@/common/constants";

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

export function listenClick() {
  document.body.addEventListener("click", e => {
    e.preventDefault();
    e.stopPropagation();
    if (context.editing || isInEditorContainer(e)) return;
    let element = context.lastHoverDomEventArgs.closeElement;
    let koobooId = element.getAttribute(KOOBOO_ID);
    var args = new SelectedDomEventArgs(element, koobooId);
    context.lastMouseEventArg = e;
    context.domChangeEvent.emit(args);
    console.log(args);
  });
}

export function emitHoverEvent(el: HTMLElement) {
  let closeElement = getCloseElement(el);
  if (closeElement == null) return;
  context.hoverDomEvent.emit(new HoverDomEventArgs(el, closeElement));
}

export function emitSelectedEvent(el: HTMLElement) {
  let element = context.lastHoverDomEventArgs.closeElement;
  let koobooId = element.getAttribute(KOOBOO_ID);
  var args = new SelectedDomEventArgs(element, koobooId);
  context.domChangeEvent.emit(args);
}
