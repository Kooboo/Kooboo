import context from "../../context";
import { isSkipHover } from "../../common/dom";
import { getKoobooInfo } from "../../common/koobooInfo";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";

export function domSelected(document: Document) {
  document.body.addEventListener("click", e => {
    e.preventDefault();
    e.stopPropagation();

    if (
      context.editing ||
      isSkipHover(e) ||
      !context.lastHoverDomEventArgs ||
      !context.lastHoverDomEventArgs.closeElement
    ) {
      return;
    }

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
