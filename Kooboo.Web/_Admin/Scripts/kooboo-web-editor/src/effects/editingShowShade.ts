import context from "../context";
import { EditorShade } from "../models/EditorShade";

let editorShade: EditorShade | undefined;

export default (document: Document) => {
  if (editorShade) return;
  editorShade = new EditorShade(document);

  context.tinymceDisplayEvent.addEventListener(display => {
    if (!context.lastSelectedDomEventArgs) return;

    if (display) {
      editorShade!.updateSource(context.lastSelectedDomEventArgs.closeElement);
    } else {
      editorShade!.clear();
    }
  });

  context.tinymceInputEvent.addEventListener(() => {
    if (!context.lastSelectedDomEventArgs) return;
    editorShade!.updateSource(context.lastSelectedDomEventArgs.closeElement);
  });
};
