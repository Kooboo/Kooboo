import { getChildNodes, isText } from "@/utils/dom";
import { getKoobooBindings, isDesignerWidget } from "../binding";

import type { Change } from "../state/change";
import { Completer } from "@/utils/lang";
import GlobalTextDialog from "./global-text-dialog.vue";
import { K_REF } from "../constants";
import type { Operation } from "../state/operation";
import { active } from "../features/edit-dom";
import { addHistory } from "../state";
import { doc } from "../page";
import { newGuid } from "@/utils/guid";
import { ref } from "vue";

export interface TextNode {
  text: Text;
  id: string;
  content: string;
}

const show = ref(false);
let completer: Completer<void>;
const list = ref<TextNode[]>([]);

const showTextDialog = async () => {
  completer = new Completer<void>();
  list.value = getTexts();
  show.value = true;
  return await completer.promise;
};

function getTexts() {
  const matches = [];

  for (const f of getChildNodes(doc.value.body!, true)) {
    if (!isText(f)) continue;
    const text = f as Text;
    const content = text.data
      .trim()
      .replaceAll("\r\n", "")
      .replaceAll("\n", "");
    if (!content) continue;
    const parentElement = text.parentElement;
    if (isDesignerWidget(parentElement)) {
      continue;
    }
    const tagName = parentElement?.tagName?.toLowerCase();
    if (tagName == "script" || tagName == "style") continue;

    matches.push({
      text: text,
      el: parentElement,
    });
  }

  const result: TextNode[] = [];

  for (const matched of matches) {
    if (!matched.el || !active(matched.el)) continue;
    const id = matched.el.getAttribute(K_REF) || newGuid();
    matched.el.setAttribute(K_REF, id);

    result.push({
      text: matched.text,
      id: id,
      content: matched.text.data,
    });
  }
  return result;
}

const close = (success: boolean, operations: Operation<Change>[]) => {
  show.value = false;
  completer.resolve();

  if (success) {
    if (operations.length) addHistory(operations);
    completer.resolve();
  } else {
    for (const operation of [...operations].reverse()) {
      operation.undo();
    }

    completer.reject();
  }

  show.value = false;
};

export { show, close, showTextDialog, GlobalTextDialog, list };
