import context from "../../common/context";
import { setInlineEditor, setImgEditor } from "./tinymce";
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

  context.floatMenuClickEvent.addEventListener(async e => {
    if (context.editing || e != MenuActions.editImage) return;
    await delay(50);
    if (context.lastSelectedDomEventArgs) {
      let selectedDom = context.lastSelectedDomEventArgs.element;
      setImgEditor(selectedDom);
    }
  });
}
