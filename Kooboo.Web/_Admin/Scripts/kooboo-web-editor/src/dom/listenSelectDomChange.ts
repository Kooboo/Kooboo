import context from "../context";
import elementAnalyze from "./elementAnalyze";
import { SelectedDomEventArgs } from "../events/selectedDomEvent";

export default (document: Document) => {
  document.body.addEventListener("mouseover", e => {
    e.stopPropagation();
    let el = e.target as HTMLElement;

    if (
      context.lastSelectedDomEventArgs &&
      context.lastSelectedDomEventArgs.element == el
    )
      return;

    let { koobooComment, koobooId, closeEl } = elementAnalyze(el);
    var args = new SelectedDomEventArgs(el, closeEl, koobooId, koobooComment);
    args.lastSelectedDomEventArgs = context.lastSelectedDomEventArgs;
    context.domChangeEvent.emit(args);
    context.lastSelectedDomEventArgs = args;
  });
};
