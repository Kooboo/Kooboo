import { isInEditorContainer } from "./utils";
import context from "@/common/context";
import { getCloseElement, getKoobooInfo } from "@/common/koobooInfo";
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

function listenHover() {
  document.body.addEventListener("mouseover", hover);
  const mouseenter = (e: MouseEvent) => {
    hover(e);
    document.body.removeEventListener("mousemove", mouseenter);
  };
  document.body.addEventListener("mousemove", mouseenter);
}

function listenClick() {
  document.body.addEventListener("click", e => {
    e.preventDefault();
    e.stopPropagation();
    if (context.editing || isInEditorContainer(e)) return;

    let { comments, koobooId, closeParent, parentKoobooId } = getKoobooInfo(
      context.lastHoverDomEventArgs.closeElement
    );

    if (comments.length == 0) return;

    var args = new SelectedDomEventArgs(
      context.lastHoverDomEventArgs.closeElement,
      koobooId,
      closeParent,
      parentKoobooId,
      comments
    );
    context.lastMouseEventArg = e;
    context.domChangeEvent.emit(args);
  });
}

export function listenDomEvents() {
  listenHover();
  listenClick();
}
