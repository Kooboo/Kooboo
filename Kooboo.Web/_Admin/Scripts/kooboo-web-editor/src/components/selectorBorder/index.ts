import context from "../../context";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";
import { HoverBorder } from "./HoverBorder";

let hoverBorder: HoverBorder;

export default (document: Document) => {
  if (hoverBorder) return;
  hoverBorder = new HoverBorder(document);

  context.hoverDomEvent.addEventListener(e => {
    if (!e.closeElement || context.editing) {
      hoverBorder.clear();
      return;
    }
    hoverBorder.updateSource(e.closeElement);
  });

  context.editableEvent.addEventListener(e => {
    if (e) {
      hoverBorder.clear();
    }
  });
};
