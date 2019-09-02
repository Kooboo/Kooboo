import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { createIframe } from "@/dom/element";
import { createItem, dataItem } from "./item";
import { editableData } from "./utils";
import { shareStyle } from "@/dom/utils";
import { createEditDataSettings } from "../richEditor/settings";

export function createDataEdtor(elements: HTMLElement[]) {
  const iframe = createIframe();
  iframe.style.height = "600px";
  const { modal, setCancelHandler, setOkHandler, close } = createModal(TEXT.EDIT_DATA, iframe);
  let result = new Promise((rs, rj) => {
    setCancelHandler(() => {
      close();
      rj();
    });

    setOkHandler(() => {
      close();
      rs();
    });
  });

  context.container.appendChild(modal);
  let doc = iframe.contentDocument!;
  initFrame(doc);
  let container = doc.body;
  let richEditorScript = document.createElement("script");
  richEditorScript.src = "\\_Admin\\Scripts\\kooboo-web-editor\\richEditor.bundle.js";
  richEditorScript.onload = () => {
    for (const i of elements) {
      const onDelete = (item: dataItem) => {
        container.removeChild(item.item);
        item.el.parentElement!.removeChild(item.el);
      };

      const onCopy = (dataItem: dataItem) => {
        let cloned = dataItem.el.cloneNode(true) as HTMLElement;
        dataItem.el.parentElement!.insertBefore(cloned, dataItem.el.nextElementSibling);
        let { item, content } = createItem(cloned, onDelete, onCopy);
        container.insertBefore(item, dataItem.item.nextElementSibling);
        (doc as any).init(createEditDataSettings(content, i));
      };

      let { item, content } = createItem(i, onDelete, onCopy);
      container.appendChild(item);
      (doc as any).init(createEditDataSettings(content, i));
    }
  };
  container.appendChild(richEditorScript);
  return result;
}

function initFrame(doc: Document) {
  let myContent = "<!DOCTYPE html><html><head></head><body></body></html>";
  doc.open("text/htmlreplace");
  doc.write(myContent);
  doc.close();
  shareStyle(doc.body);
}
