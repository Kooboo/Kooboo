import { Settings, Editor, dom } from "tinymce";
import context from "../../common/context";
import { setLang, onBlur, onSetContent, onKeyDown, onBeforeSetContent, getToolbar, initInstanceCallback, savePluginCallback } from "./utils";
import { createLinkPicker } from "../linkPicker";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { STANDARD_Z_INDEX } from "@/common/constants";
import { delay } from "@/common/utils";
import conetxt from "@/common/context";

export interface SetInlineEditorArgs {
  selector: HTMLElement;
  onSave: () => void;
  onCancel: () => void;
}

function createCommonSettings(target: HTMLElement) {
  let settings = {
    target: target,
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
    browser_spellcheck: true,
    forced_root_block: "",
    visual_anchor_class: "no-anchor",
    valid_elements: "*[*]",
    valid_children: "*[*]",
    plugins: ["link", "image"],
    valid_styles:
      "width,height,color,font-size,font-family,background,background-color,background-image,font-weight,font-style,text-decoration,float,margin,margin-top,margin-right,margin-bottom,margin-left,display,text-align",
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
  (target as any)._position = target.style.position;
  setLang(settings);

  return settings;
}

export function createSettings(args: SetInlineEditorArgs) {
  const { selector, onSave, onCancel } = args;
  const settings = createCommonSettings(selector);
  settings.toolbar = getToolbar(selector);
  settings.plugins = ["save", "link", "image"];
  settings.init_instance_callback = initInstanceCallback;
  settings.setup = (editor: Editor) => {
    editor.on("Blur", onBlur);
    editor.once("SetContent", onSetContent);
    editor.on("Change", () => context.tinymceInputEvent.emit());
    editor.on("KeyUp", () => context.tinymceInputEvent.emit());
    editor.on("KeyDown", onKeyDown);
    editor.on("BeforeSetContent", onBeforeSetContent);
  };
  (settings as any).save_enablewhendirty = false;
  (settings as any).save_onsavecallback = (e: Editor) => savePluginCallback(e, onSave);
  (settings as any).save_oncancelcallback = (e: Editor) => savePluginCallback(e, onCancel);
  return settings;
}

export function createEditDataSettings(target: HTMLElement, source: HTMLElement) {
  const settings = createCommonSettings(target);
  settings.toolbar = "undo redo | bold italic underline | forecolor backcolor | fontselect fontsizeselect | image | link unlink";
  settings.plugins = ["link", "image"];
  settings.setup = (editor: Editor) => {
    editor.once("SetContent", onSetContent);
    editor.on("KeyDown", onKeyDown);
    editor.on("BeforeSetContent", onBeforeSetContent);
    editor.on("Focus", async (e: any) => {
      let container = editor.getContainer() as HTMLElement;
      container.style.zIndex = STANDARD_Z_INDEX + 6 + "";

      await delay(100);
      if (container.nextElementSibling instanceof HTMLElement) {
        container.nextElementSibling.style.zIndex = STANDARD_Z_INDEX + 7 + "";
      }
    });
    editor.on("Change", (e: any) => {
      source.innerHTML = target.innerHTML;
    });
  };
  return settings;
}
