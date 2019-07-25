import { TEXT } from "@/common/lang";
import { createModal } from "../modal";
import { createLabelInput, createDiv } from "@/dom/element";
import { getEditorContainer } from "@/dom/utils";

export function createFormEditor(el: HTMLFormElement) {
  let container = createDiv();
  let action = createLabelInput("action", "80px");
  action.setContent(el.getAttribute("action")!);
  action.setInputHandler(content => {
    el.action = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(action.input);
  let method = createLabelInput("method", "80px");
  method.setContent(el.getAttribute("method")!);
  method.setInputHandler(content => {
    el.method = (content.target! as HTMLInputElement).value;
  });
  container.appendChild(method.input);
  let { modal, setOkHandler, setCancelHandler, close } = createModal(TEXT.EDIT_FORM, container, "400px");
  getEditorContainer().appendChild(modal);
  return new Promise((rs, rj) => {
    setOkHandler(() => {
      rs();
      close();
    });
    setCancelHandler(() => {
      rj();
      close();
    });
  });
}
