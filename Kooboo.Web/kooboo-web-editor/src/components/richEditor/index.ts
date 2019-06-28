import context from "../../common/context";
import { setInlineEditor } from "./tinymce";
import { MenuActions } from "../../events/FloatMenuClickEvent";

export function registerInlineEditor() {
  context.floatMenuClickEvent.addEventListener(e => {
    if (context.editing || e != MenuActions.edit) return;
    if (context.lastSelectedDomEventArgs) {
      let selectedDom = context.lastSelectedDomEventArgs.element;
      setInlineEditor(selectedDom);
    }
  });
}
