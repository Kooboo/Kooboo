import { init, Settings } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import "tinymce/plugins/link";
import "tinymce/plugins/image";
import { createSettings, SetInlineEditorArgs } from "./settings";

async function createEditor(settings: Settings) {
  let selector = settings.target as HTMLElement;
  init(settings);
  if (selector instanceof HTMLElement) selector.focus();
}

export async function setInlineEditor(args: SetInlineEditorArgs) {
  let settings = createSettings(args);
  createEditor(settings);
}
