import { init, Settings } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import "tinymce/plugins/link";
import "tinymce/plugins/image";
import { createSettings } from "./settings";

async function createEditor(settings: Settings) {
  let selector = settings.target as HTMLElement;
  init(settings);
  if (selector instanceof HTMLElement) selector.focus();
}

export async function setInlineEditor(selector: HTMLElement, startContent?: string) {
  return new Promise((rs, rj) => {
    let settings = createSettings(selector, rj, rs);
    createEditor(settings).then(e => {
      if (startContent) {
        (e as any)._content = startContent;
        (e as any)._isReplace = true;
      }
    });
  });
}
