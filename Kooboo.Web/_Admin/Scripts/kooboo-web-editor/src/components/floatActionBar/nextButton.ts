import { createButton } from "./button";
import nextIcon from "../../assets/icons/xiayibu.svg";
import OperationManager from "../../OperationManager";
import { createNotice } from "./Notice";
import context from "../../context";

export function createNextButton(document: Document) {
  var nextBtn = createButton(document, nextIcon);
  nextBtn.onclick = () => OperationManager.next();
  var nextNotice = createNotice(document);
  nextBtn.appendChild(nextNotice);
  context.operationEvent.addEventListener(e => {
    nextNotice.setCount(e.backupOperationCount);
  });
  return nextBtn;
}
