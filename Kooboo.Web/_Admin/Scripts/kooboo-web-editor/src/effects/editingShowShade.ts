import context from "../context";
import { EditorShade } from "../models/editorShade";

let editorShade: EditorShade | undefined;

export default (document: Document) => {
  if (editorShade) return;
  editorShade = new EditorShade(document);

  context.tinymceDisplayEvent.addEventListener(display => {
    if (display) {
      editorShade!.updateSource(context.lastSelectedDomEventArgs!.closeElement);
    } else {
      editorShade!.clear(document);
    }
  });

  context.tinymceInputEvent.addEventListener(() => {
    editorShade!.updateSource(context.lastSelectedDomEventArgs!.closeElement);
  });
};
