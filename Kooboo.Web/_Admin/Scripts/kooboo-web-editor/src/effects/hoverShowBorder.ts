import context from "../context";
import { SelectedDomEventArgs } from "../events/selectedDomEvent";
import { HoverBorder } from "../models/hoverBorder";

let hoverBorder: HoverBorder | undefined;

export default (document: Document) => {
  if (hoverBorder) return;
  hoverBorder = new HoverBorder(document);

  context.domChangeEvent.addEventListener((e: SelectedDomEventArgs) => {
    if (context.editing) return;
    hoverBorder!.updateSource(e.closeElement);
  });

  context.tinymceDisplayEvent.addEventListener(e => {
    if (e) {
      hoverBorder!.clear(document);
    }
  });
};
