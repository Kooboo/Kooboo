import tinymce from "tinymce-declaration";
import { createSettings, SetInlineEditorArgs } from "./settings";

async function createEditor(settings: tinymce.Settings) {
  let selector = settings.target as HTMLElement;
  tinymce.init(settings);
  if (selector instanceof HTMLElement) selector.focus();
}

export async function setInlineEditor(args: SetInlineEditorArgs) {
  let settings = createSettings(args);
  createEditor(settings);
}
