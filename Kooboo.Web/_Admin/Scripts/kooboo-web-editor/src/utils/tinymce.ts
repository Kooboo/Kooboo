import tinymce, { EditorManager, Settings, Editor } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import context from "../context";

export async function inlineSimple(selector: Element) {
  if ((selector as any)._tinymceeditor) return;
  EditorManager.editors.forEach(i => {
    (i.getElement() as any)._tinymceeditor = null;
    i.remove();
  });

  //fix tinymce known issue https://github.com/tinymce/tinymce/issues/2453
  let liContentBackup = null;
  if (selector.tagName == "LI") liContentBackup = selector.innerHTML;

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
    remove_trailing_brs: false,
    forced_root_block: "",
    valid_elements: "*[*]",
    valid_children: "*[*]",
    valid_styles: "*[*]",
    plugins: ["save"],
    toolbar:
      "save cancel | bold italic forecolor fontselect fontsizeselect | image link unlink",
    init_instance_callback: e => {
      context.editing = true;
      context.tinymceEvent.emit(context.editing);
    },
    setup: e => {
      e.on("Blur", () => {
        context.editing = false;
        context.tinymceEvent.emit(context.editing);
      });
    }
  } as Settings;
  (settings as any).save_oncancelcallback = (e: Editor) => {
    e.hide();
  };
  let editor = await EditorManager.init(settings);
  if (liContentBackup) selector.innerHTML = liContentBackup;
  if (editor instanceof Array) editor = editor[0];
  let container = editor.getContainer();
  console.dir(container);
  if (container instanceof HTMLElement) container.style.zIndex = "9999";
  (selector as any)._tinymceeditor = editor;
  if (selector instanceof HTMLElement) selector.focus();
}
