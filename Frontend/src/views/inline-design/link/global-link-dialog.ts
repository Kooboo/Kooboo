import type { Change } from "../state/change";
import { Completer } from "@/utils/lang";
import GlobalLinkDialog from "./global-link-dialog.vue";
import { K_REF } from "../constants";
import type { Operation } from "../state/operation";
import { active } from "../features/edit-link";
import { addHistory } from "../state";
import { doc } from "../page";
import { getElements } from "@/utils/dom";
import { isDesignerWidget } from "../binding";
import { newGuid } from "@/utils/guid";
import { ref } from "vue";

export interface Link {
  url: string;
  id: string;
  content: string;
}

const show = ref(false);
let completer: Completer<void>;
const list = ref<Link[]>([]);

const showLinkDialog = async () => {
  completer = new Completer<void>();
  list.value = getLinks();
  show.value = true;
  return await completer.promise;
};

function getLinks() {
  const elements = getElements(doc.value!).filter(
    (it) => !isDesignerWidget(it)
  );
  const result: Link[] = [];

  for (const element of elements) {
    if (!active(element)) continue;
    const id = newGuid();
    element.setAttribute(K_REF, id);

    result.push({
      url: element.getAttribute("href")!,
      id: id,
      content: element.textContent || element.innerText || "",
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

export { show, close, showLinkDialog, GlobalLinkDialog, list };
