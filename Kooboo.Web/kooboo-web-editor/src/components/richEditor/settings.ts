import { Settings, Editor } from "tinymce";
import context from "../../common/context";
import { pickImg } from "../../common/outsideInterfaces";
import {
  setLang,
  save_oncancelcallback,
  save_onsavecallback,
  onBlur,
  onSetContent,
  onRemove,
  onKeyDown,
  onBeforeSetContent,
  getToolbar
} from "./utils";
import { createLinkPicker } from "../linkPicker";

export function createSettings(selector: HTMLElement) {
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
    toolbar: getToolbar(selector),
    init_instance_callback: e => (context.editing = true),
    setup(editor: Editor) {
      editor.on("Blur", onBlur);
      editor.once("SetContent", onSetContent);
      editor.on("Remove", onRemove);
      editor.on("Change", () => context.tinymceInputEvent.emit());
      editor.on("KeyUp", () => context.tinymceInputEvent.emit());
      editor.on("KeyDown", onKeyDown);
      editor.on("BeforeSetContent", onBeforeSetContent);
    },
    async file_picker_callback(callback, value, meta: any) {
      if (meta.filetype == "image") {
        pickImg(path => {
          callback(path, {});
        });
      } else {
        try {
          let newValue = await createLinkPicker(value);
          callback(newValue, {});
        } catch (error) {}
      }
    }
  } as Settings;
  //fix https://github.com/tinymce/tinymce/issues/1828
  (selector as any)._isRelative = selector.style.position == "relative";
  setLang(settings);
  (settings as any).save_enablewhendirty = false;
  (settings as any).save_oncancelcallback = save_oncancelcallback;
  (settings as any).save_onsavecallback = save_onsavecallback;
  return settings;
}

export function createImgSettings(selector: HTMLElement) {
  let setting = createSettings(selector);
  setting.plugins = ["save", "image"];
  setting.toolbar = "save cancel | image";

  return setting;
}
