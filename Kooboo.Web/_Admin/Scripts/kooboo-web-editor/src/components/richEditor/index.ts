import context from "../../context";
import { setInlineEditor, setImgEditor } from "./tinymce";
import { MenuActions } from "../../events/FloatMenuClickEvent";

export function registerInlineEditor() {
  context.floatMenuClickEvent.addEventListener(e => {
    if (context.editing || e != MenuActions.edit) return;
    if (context.lastSelectedDomEventArgs) {
      let selectedDom = context.lastSelectedDomEventArgs.element;
      setInlineEditor(selectedDom);
    }
  });

  context.floatMenuClickEvent.addEventListener(e => {
    if (context.editing || e != MenuActions.editImage) return;
    if (context.lastSelectedDomEventArgs) {
      let selectedDom = context.lastSelectedDomEventArgs.element;
      setImgEditor(selectedDom);
    }
  });
}
