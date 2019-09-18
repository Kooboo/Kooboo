import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import context from "@/common/context";
import { createIframe } from "@/dom/element";
import { createItem, dataItem } from "./item";
import { shareStyle } from "@/dom/utils";
import { createEditDataSettings } from "../richEditor/settings";

export function createDataEdtor(elements: HTMLElement[]) {
  const iframe = createIframe();
  iframe.style.height = "600px";
  let result = addModal(iframe);
  let doc = iframe.contentDocument!;
  let win = iframe.contentWindow!;
  initDoc(doc);
  doc.head.appendChild(createScript(win, elements));
  return result;
}

function initDoc(doc: Document) {
  let myContent = "<!DOCTYPE html><html><head></head><body></body></html>";
  doc.open("text/htmlreplace");
  doc.write(myContent);
  doc.close();
  shareStyle(doc.body);
}

function addModal(el: HTMLElement) {
  const { modal, setCancelHandler, setOkHandler, close } = createModal(TEXT.EDIT_DATA, el);

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
  return result;
}

function createScript(win: Window, elements: HTMLElement[]) {
  let container = win.document.body;
  let richEditorScript = document.createElement("script");
  richEditorScript.src = "\\_Admin\\Scripts\\kooboo-web-editor\\richEditor.min.js";
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
        (win as any).tinymce.init(createEditDataSettings(content, cloned));
      };

      let { item, content } = createItem(i, onDelete, onCopy);
      container.appendChild(item);
      (win as any).tinymce.init(createEditDataSettings(content, i));
    }
  };

  return richEditorScript;
}
