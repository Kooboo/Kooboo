import { createButton } from "./button";
import preIcon from "../../assets/icons/shangyibu.svg";
import preEnableIcon from "../../assets/icons/shangyibu_enable.svg";
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
    preBtn.changeIcon(e.operationCount > 0 ? preEnableIcon : preIcon);
  });
  return preBtn;
}
