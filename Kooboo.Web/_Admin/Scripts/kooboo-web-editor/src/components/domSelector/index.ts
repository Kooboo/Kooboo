import { isSkipHover, getKoobooInfo } from "../../helpers/domAnalyze";
import context from "../../context";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";

export function registerDomSelector(document: Document) {
  document.body.addEventListener("mouseover", e => {
    e.stopPropagation();

    if (isSkipHover(e) || context.editing) return;

    let el = e.target as HTMLElement;

    if (
      context.lastSelectedDomEventArgs &&
      context.lastSelectedDomEventArgs.element == el
    )
      return;

    let { comments, koobooId, closeElement } = getKoobooInfo(el);
    var args = new SelectedDomEventArgs(el, closeElement, koobooId, comments);
    context.domChangeEvent.emit(args);
    context.lastSelectedDomEventArgs = args;
  });
}
