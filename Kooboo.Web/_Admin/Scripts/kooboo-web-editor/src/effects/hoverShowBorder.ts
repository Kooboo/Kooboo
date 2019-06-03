import context from "../context";
import { SelectedDomEventArgs } from "../events/selectedDomEvent";
import { HoverBorder } from "../models/hoverBorder";

export default (document: Document) => {
  const hoverBorder = new HoverBorder(document);

  context.domChangeEvent.addEventListener((e: SelectedDomEventArgs) => {
    if (context.editing) return;
    hoverBorder.updateSource(e.closeElement);
  });

  context.tinymceEvent.addEventListener(e => {
    if (e) {
      hoverBorder.clear(document);
    }
  });
};
