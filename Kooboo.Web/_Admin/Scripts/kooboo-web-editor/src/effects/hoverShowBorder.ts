import context from "../context";
import { SelectedDomEventArgs } from "../models/selectedDomEvent";

const HOVER_OUTLINE = "#1fb5f6a3 dotted 2px";

const hoverShowBorderHandler = (e: SelectedDomEventArgs) => {
  if (e.lastSelectedDomEventArgs && e.lastSelectedDomEventArgs.closeElement) {
    var element = e.lastSelectedDomEventArgs.closeElement;
    element.style.outline = (element as any)._outline;
  }
  (e.closeElement as any)._outline = e.closeElement.style.outline;
  e.closeElement.style.outline = HOVER_OUTLINE;
};

export default () => {
  if (context.domChangeEvent.handlers.some(s => s == hoverShowBorderHandler))
    return;
  context.domChangeEvent.addEventListener(hoverShowBorderHandler);
};
