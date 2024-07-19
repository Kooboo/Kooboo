import { ref, triggerRef } from "vue";
import InlineEditor from "./inline-editor.vue";
import { Completer } from "@/utils/lang";
import { CONTENT_EDITABLE } from "../constants";
import { currentElement, win } from "../page";

const editing = ref(false);
let completer: Completer<void>;

async function startEdit(el: HTMLElement) {
  if (editing.value) return;
  el.setAttribute(CONTENT_EDITABLE, "true");
  (el as any).onclick_bak = el.onclick;

  el.onclick = (e) => {
    e.stopPropagation();
    e.preventDefault();
  };

  const observer = new MutationObserver(() => {
    triggerRef(currentElement);
  });
  observer.observe(el, {
    attributes: true,
    childList: true,
    subtree: true,
    characterData: true,
  });
  editing.value = true;
  completer = new Completer();

  try {
    await completer.promise;
  } finally {
    el.removeAttribute(CONTENT_EDITABLE);
    el.onclick = (el as any).onclick_bak;
    observer.disconnect();
    editing.value = false;
  }
}

function endEdit(isCancel: boolean) {
  if (isCancel) {
    completer.reject();
  } else {
    completer?.resolve();
  }

  const selection = win.value.getSelection();
  if (!selection) return;
  if ("empty" in selection) selection.empty();
  if ("removeAllRanges" in selection) selection.removeAllRanges();
}

export { editing, startEdit, endEdit, InlineEditor };
