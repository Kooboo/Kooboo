import { save_onsavecallback, save_oncancelcallback } from "./savePluginEvent";
import { Settings } from "tinymce";
import { lang } from "../../lang";
import { getAllElement } from "../../common/dom";
import context from "../../context";

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
    setup: editor => {
      editor.on("Blur", () => false);
      editor.once("SetContent", e => {
        e.target._content = e.content;
        var targetElm = e.target.targetElm as HTMLElement;
        for (const element of getAllElement(targetElm, true)) {
          if (
            (element.tagName.toLowerCase() == "i" ||
              element.tagName.toLowerCase() == "a") &&
            element.innerHTML.indexOf("<!--i-->") == -1
          ) {
            element.innerHTML += "<!--i-->";
            console.log(element, element.innerHTML);
          }
        }
      });
      editor.on("Remove", e => {
        let element = e.target.getElement();
        element._tinymceeditor = null;

        if (!element._isRelative) {
          element.style.position = "";
        }

        if (element instanceof HTMLElement) {
          if (element.id.startsWith("mce_")) element.removeAttribute("id");
          if (element.getAttribute("style") == "")
            element.removeAttribute("style");
        }
      });
      editor.on("Change", () => context.tinymceInputEvent.emit());
      editor.on("KeyUp", () => context.tinymceInputEvent.emit());
      editor.on("KeyDown", e => {
        var targetElm = e.target as HTMLElement;
        if (e.code == "Backspace" && targetElm.innerHTML == "<!--i-->")
          return false;
      });
      editor.on("BeforeSetContent", function(e) {
        //fix tinymce known issue https://github.com/tinymce/tinymce/issues/2453
        var targetElm = e.target.targetElm as HTMLElement;
        if (targetElm.tagName.toLowerCase() == "li") {
          if (targetElm.children.length == 0) {
            e.content = targetElm.textContent;
          } else if (e.content === 0) {
            e.content = targetElm.innerHTML;
          }
        }

        e.format = "raw";
      });
    },
    file_picker_callback(callback, value, meta: any) {
      if (meta.filetype == "image") {
        callback("myimage.jpg", { alt: "My alt text" });
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
