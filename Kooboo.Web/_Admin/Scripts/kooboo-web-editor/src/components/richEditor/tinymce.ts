import { EditorManager, Settings } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import "tinymce/plugins/link";
import "tinymce/plugins/image";
import { createSettings, createImgSettings } from "./settings";
import { setZIndex } from "./editorUtils";
import { EMPTY_COMMENT } from "../../constants";

async function createEditor(settings: Settings) {
  let selector = settings.target as HTMLElement;
  let editor = await EditorManager.init(settings);
  if (editor instanceof Array) editor = editor[0];
  setZIndex(editor);
  if (selector instanceof HTMLElement) selector.focus();
  return editor;
}

export async function setInlineEditor(selector: HTMLElement) {
  let settings = createSettings(selector);
  await createEditor(settings);
}

export async function setImgEditor(selector: HTMLElement) {
  (selector as any)._display = selector.style.display;
  selector.style.display = "block";

  let settings = createImgSettings(selector);
  let editor = await createEditor(settings);
  editor.setContent(EMPTY_COMMENT);
  editor.execCommand("mceImage");
  (editor as any)._onremove = () => {
    console.dir(selector);
    if ((selector as any)._display != undefined) {
      selector.style.display = (selector as any)._display;
      (selector as any)._display = undefined;
    }
  };
}
