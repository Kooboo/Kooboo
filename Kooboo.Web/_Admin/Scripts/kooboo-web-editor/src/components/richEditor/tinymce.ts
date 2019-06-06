import { EditorManager, Settings, Editor } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import "tinymce/plugins/link";
import "tinymce/plugins/image";
import context from "../../context";
import { STANDARD_Z_INDEX } from "../../constants";
import { Operation } from "../../models/Operation";

export async function setInlineEditor(selector: Element) {
  if ((selector as any)._tinymceeditor) return;
  EditorManager.editors.forEach(i => {
    (i.getElement() as any)._tinymceeditor = null;
    i.remove();
  });

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
      editor.once("SetContent", e => (e.target._content = e.content));
      editor.on("Remove", e => (e.target.getElement()._tinymceeditor = null));
      editor.on("Change", e => context.tinymceInputEvent.emit());
      editor.on("KeyUp", e => context.tinymceInputEvent.emit());
      editor.on("BeforeSetContent", function(e) {
        //fix tinymce known issue https://github.com/tinymce/tinymce/issues/2453
        var targetElm = e.target.targetElm;
        if (targetElm.tagName.toLowerCase() == "li") {
          if (targetElm.children.length == 0) {
            e.content = targetElm.textContent;
          } else if (e.content === 0) {
            e.content = targetElm.innerHTML;
          }
        }
        e.format = "raw";
      });
    }
  } as Settings;
  (settings as any).save_enablewhendirty = false;
  (settings as any).save_oncancelcallback = (e: Editor) => {
    e.setContent((e as any)._content);
    e.remove();
    context.editing = false;
  };

  (settings as any).save_onsavecallback = (e: Editor) => {
    let startContent = (e as any)._content;
    let endContent = e.getContent();
    let element = e.getElement() as HTMLElement;
    let comment = context.lastSelectedDomEventArgs!.koobooComment;
    let koobooId = context.lastSelectedDomEventArgs!.koobooId;
    let operation = new Operation(
      element,
      startContent,
      endContent,
      comment,
      koobooId
    );
    context.operationManager.add(operation);
    e.remove();
    context.editing = false;
  };

  let editor = await EditorManager.init(settings);
  if (editor instanceof Array) editor = editor[0];
  let container = editor.getContainer();
  if (container instanceof HTMLElement) {
    container.style.zIndex = STANDARD_Z_INDEX + 1 + "";
    setTimeout(() => {
      if (container.nextElementSibling instanceof HTMLElement) {
        container.nextElementSibling.style.zIndex = STANDARD_Z_INDEX + 2 + "";
      }
    }, 500);
  }
  (selector as any)._tinymceeditor = editor;
  if (selector instanceof HTMLElement) selector.focus();
}
