import context from "../context";
import { SelectedDomEventArgs } from "../events/selectedDomEvent";
import { EditorShade } from "../models/editorShade";

export default (document: Document) => {
  const editorShade = new EditorShade(document);
  context.tinymceEvent.addEventListener(e => {
    if (e) {
      console.log(context.lastSelectedDomEventArgs!.closeElement);
      editorShade.updateSource(context.lastSelectedDomEventArgs!.closeElement);
    } else {
      // editorShade.clear(document);
    }
  });
};
