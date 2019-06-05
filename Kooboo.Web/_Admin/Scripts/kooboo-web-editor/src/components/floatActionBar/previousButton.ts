import { createButton } from "./button";
import preIcon from "../../assets/icons/shangyibu.svg";
import OperationManager from "../../OperationManager";
import { createNotice } from "./Notice";
import context from "../../context";

export function createPreviousButton(document: Document) {
  var preBtn = createButton(document, preIcon);
  preBtn.onclick = () => OperationManager.previous();
  var preNotice = createNotice(document);
  preBtn.appendChild(preNotice);
  context.operationEvent.addEventListener(e => {
    preNotice.setCount(e.operationCount);
  });
  return preBtn;
}
