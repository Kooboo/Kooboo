import context from "../../common/context";
import { setInlineEditor } from "./tinymce";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { delay } from "../../common/utils";

export function registerInlineEditor() {
  context.floatMenuClickEvent.addEventListener(e => {
    if (context.editing || e != MenuActions.edit) return;
    if (context.lastSelectedDomEventArgs) {
      let selectedDom = context.lastSelectedDomEventArgs.element;
      setInlineEditor(selectedDom);
    }
  });
}
