import context from "../context";
import { SelectedDomEventArgs } from "../models/selectedDomEvent";
import hoverBorder from "../models/hoverBorder";

const hoverShowBorderHandler = (e: SelectedDomEventArgs) => {
  if (!context.editing) {
    hoverBorder.updateSource(e.closeElement);
  }
};

export default () => {
  if (context.domChangeEvent.handlers.some(s => s == hoverShowBorderHandler))
    return;
  context.domChangeEvent.addEventListener(hoverShowBorderHandler);
};
