import context from "../context";
import { setInlineEditor } from "../components/tinymceEditor";
import { isSkipHover } from "../dom/domAnalyze";

export default (document: Document) => {
  document.body.addEventListener("click", e => {
    if (context.editing || isSkipHover(e)) return;
    if (context.lastSelectedDomEventArgs) {
      let selectedDom = context.lastSelectedDomEventArgs.closeElement;
      // if (selectedDom.innerHTML.indexOf("kooboo-id") != -1) return;
      setInlineEditor(selectedDom);
    }
  });
};
