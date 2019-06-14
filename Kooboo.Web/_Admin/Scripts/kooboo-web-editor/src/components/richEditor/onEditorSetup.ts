import { Editor } from "tinymce";
import { getAllElement } from "../../common/dom";
import context from "../../context";

export function onEditorSetup(editor: Editor) {
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
      if (element.getAttribute("style") == "") element.removeAttribute("style");
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
}
