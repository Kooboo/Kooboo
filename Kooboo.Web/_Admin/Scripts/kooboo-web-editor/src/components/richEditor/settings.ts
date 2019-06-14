import { save_onsavecallback, save_oncancelcallback } from "./savePluginEvent";
import { Settings } from "tinymce";
import { lang } from "../../lang";
import context from "../../context";
import { pickImg } from "../../common/koobooInterfaces";
import { onEditorSetup } from "./onEditorSetup";

export function createSettings(selector: Element) {
  const settings = {
    target: selector,
    inline: true,
    hidden_input: false,
    skin_url: "_Admin\\Styles\\kooboo-web-editor\\tinymce\\ui\\oxide",
    menubar: false,
    element_format: "html",
    allow_script_urls: true,
    style_formats_merge: false,
    style_formats_autohide: false,
    allow_conditional_comments: true,
    allow_html_in_named_anchor: true,
    allow_unsafe_link_target: true,
    convert_fonts_to_spans: false,
    remove_trailing_brs: true,
    browser_spellcheck: false,
    forced_root_block: "",
    valid_elements: "*[*]",
    valid_children: "*[*]",
    valid_styles: "*[*]",
    plugins: ["save", "link", "image"],
    toolbar:
      "save cancel | undo redo | bold italic forecolor fontselect fontsizeselect | image link unlink",
    init_instance_callback: e => {
      context.editing = true;
    },
    setup: onEditorSetup,
    file_picker_callback(callback, value, meta: any) {
      if (meta.filetype == "image") {
        pickImg((selected: any) => {
          callback(selected.thumbnail, { alt: "" });
        });
      }
    }
  } as Settings;

  if (lang == "zh") {
    settings.language = "zh_CN";
    settings.language_url = `_Admin\\Scripts\\kooboo-web-editor\\dist\\${
      settings.language
    }.js`;
  }

  (settings as any).save_enablewhendirty = false;
  (settings as any).save_oncancelcallback = save_oncancelcallback;
  (settings as any).save_onsavecallback = save_onsavecallback;
  return settings;
}
