import { doc, win } from "../page";

import type { Change } from "../state/change";
import type { Color } from "@/global/color";
import ColorDialog from "./color-dialog.vue";
import { Completer } from "@/utils/lang";
import type { Operation } from "../state/operation";
import { addHistory } from "../state";
import { getChildElements } from "@/utils/dom";
import { getColors } from "@/global/color";
import { isDesignerWidget } from "../binding";
import { ref } from "vue";

const show = ref(false);
let completer: Completer<void>;
const colors = ref<Color[]>([]);

const showColorDialog = async (el: HTMLElement) => {
  if (!el) return;
  completer = new Completer<void>();
  show.value = true;
  colors.value = getEditableColors(el);
  return await completer.promise;
};

const close = (success: boolean, operations: Operation<Change>[]) => {
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

function getEditableColors(el: HTMLElement) {
  const elements = getChildElements(el, true, true).filter(
    (it) => !isDesignerWidget(it)
  );
  const items = getColors(doc.value, win.value, elements);

  const list: Color[] = [];
  for (const item of items) {
    switch (item.type) {
      case "file":
        list.push(item);
        break;

      case "embedded":
      case "inline":
        list.push(item);
        break;

      default:
        break;
    }
  }

  return list;
}

export { show, close, showColorDialog, ColorDialog, colors, getEditableColors };
