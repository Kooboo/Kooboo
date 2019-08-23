import { Settings, Editor } from "tinymce";
import context from "../../common/context";
import { setLang, onBlur, onSetContent, onKeyDown, onBeforeSetContent, getToolbar, initInstanceCallback, savePluginCallback } from "./utils";
import { createLinkPicker } from "../linkPicker";
import { pickImg } from "@/kooboo/outsideInterfaces";

export interface SetInlineEditorArgs {
  selector: HTMLElement;
  onSave: () => void;
  onCancel: () => void;
}

export function createSettings(args: SetInlineEditorArgs) {
  const { selector, onSave, onCancel } = args;
  const settings = {
    target: selector,
    inline: true,
    hidden_input: false,
    skin_url: `${location.origin}\\_Admin\\Styles\\kooboo-web-editor\\tinymce\\ui\\oxide`,
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
    visual_anchor_class: "no-anchor",
    valid_elements: "*[*]",
    valid_children: "*[*]",
    valid_styles:
      "width,height,color,font-size,font-family,background,background-color,background-image,font-weight,font-style,text-decoration,float,margin,margin-top,margin-right,margin-bottom,margin-left,display,text-align",
    plugins: ["save", "link", "image"],
    toolbar: getToolbar(selector),
    init_instance_callback: initInstanceCallback,
    setup(editor: Editor) {
      editor.on("Blur", onBlur);
      editor.once("SetContent", onSetContent);
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
  (selector as any)._position = selector.style.position;
  setLang(settings);
  (settings as any).save_enablewhendirty = false;
  (settings as any).save_onsavecallback = (e: Editor) => savePluginCallback(e, onSave);
  (settings as any).save_oncancelcallback = (e: Editor) => savePluginCallback(e, onCancel);
  return settings;
}
