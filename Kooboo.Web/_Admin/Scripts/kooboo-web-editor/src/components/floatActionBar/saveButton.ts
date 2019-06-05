import saveIcon from "../../assets/icons/baocun.svg";
import saveEnableIcon from "../../assets/icons/baocun_enable.svg";
import { createButton } from "./button";
import context from "../../context";

export function createSaveButton(document: Document) {
  var saveBtn = createButton(document, saveIcon);
  context.operationEvent.addEventListener(e => {
    saveBtn.changeIcon(e.operationCount > 0 ? saveEnableIcon : saveIcon);
  });
  return saveBtn;
}
