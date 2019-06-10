import saveIcon from "../../assets/icons/baocun.svg";
import saveEnableIcon from "../../assets/icons/baocun_enable.svg";
import { createButton } from "./button";
import context from "../../context";
import { OperationLogItem } from "../../models/OperationLog";
import updateOperation from "../../api/updateOperation";

export function createSaveButton(document: Document) {
  var saveBtn = createButton(document, saveIcon);
  context.operationEvent.addEventListener(e => {
    saveBtn.changeIcon(e.operationCount > 0 ? saveEnableIcon : saveIcon);
  });
  saveBtn.onclick = async () => {
    console.log("aa'");
    let operations = context.operationManager.operations;
    if (operations.length == 0) return;
    let logs = operations.map(m => {
      let log = new OperationLogItem();
      log.action = "update";
      log.editorType = "dom";
      log.koobooId = m.koobooId ? m.koobooId : "";
      log.nameOrId = m.koobooComment.nameorid!;
      log.objectType = m.koobooComment.objecttype!;
      log.value = m.newInnerHTML;
      return log;
    });
    console.log(context.operationManager, logs);
    await updateOperation(logs);
    parent.location.reload();
  };
  return saveBtn;
}
