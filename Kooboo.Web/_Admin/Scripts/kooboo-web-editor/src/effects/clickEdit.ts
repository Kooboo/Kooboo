import context from "../context";
import { inlineSimple } from "../utils/tinymce";

export default (document: Document) => {
  document.body.addEventListener("click", e => {
    if (context.editing) return;
    if (context.lastSelectedDomEventArgs) {
      let selectedDom = context.lastSelectedDomEventArgs.closeElement;
      if (selectedDom.innerHTML.indexOf("kooboo-id") != -1) return;
      inlineSimple(selectedDom);
    }
  });
};
