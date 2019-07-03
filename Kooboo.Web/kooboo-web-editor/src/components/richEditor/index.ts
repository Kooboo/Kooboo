import { EditorManager, Settings } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import "tinymce/plugins/link";
import "tinymce/plugins/image";
import { createSettings } from "./settings";
import { impoveEditorUI } from "./utils";

async function createEditor(settings: Settings) {
  let selector = settings.target as HTMLElement;
  let editor = await EditorManager.init(settings);
  if (editor instanceof Array) editor = editor[0];
  impoveEditorUI(editor);
  if (selector instanceof HTMLElement) selector.focus();
  return editor;
}

export async function setInlineEditor(selector: HTMLElement) {
  let settings = createSettings(selector);
  await createEditor(settings);
}
