import { EditorManager } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import "tinymce/plugins/link";
import "tinymce/plugins/image";
import { STANDARD_Z_INDEX } from "../../constants";
import { createSettings } from "./settings";

export async function setInlineEditor(selector: Element) {
  if ((selector as any)._tinymceeditor) return;
  EditorManager.editors.forEach(i => {
    (i.getElement() as any)._tinymceeditor = null;
    i.remove();
  });

  let settings = createSettings(selector);
  let editor = await EditorManager.init(settings);
  if (editor instanceof Array) editor = editor[0];
  let container = editor.getContainer();
  if (container instanceof HTMLElement) {
    container.style.zIndex = STANDARD_Z_INDEX + 1 + "";
    setTimeout(() => {
      if (container.nextElementSibling instanceof HTMLElement) {
        container.nextElementSibling.style.zIndex = STANDARD_Z_INDEX + 2 + "";
      }
    }, 500);
  }
  (selector as any)._tinymceeditor = editor;
  if (selector instanceof HTMLElement) selector.focus();
}
