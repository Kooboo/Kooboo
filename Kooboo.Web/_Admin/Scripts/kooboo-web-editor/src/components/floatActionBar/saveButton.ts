import saveIcon from "../../assets/icons/baocun.svg";
import saveEnableIcon from "../../assets/icons/baocun_enable.svg";
import { createButton } from "./button";
import context from "../../context";
import { OperationLogItem } from "../../models/OperationLog";
import updateOperation from "../../api/updateOperation";
import { OBJECT_TYPE } from "../../constants";
import { cleanKoobooInfo } from "../../common/koobooInfo";

export function createSaveButton(document: Document) {
  var saveBtn = createButton(document, saveIcon);
  context.operationEvent.addEventListener(e => {
    saveBtn.changeIcon(e.operationCount > 0 ? saveEnableIcon : saveIcon);
  });
  saveBtn.onclick = async () => {
    let operations = context.operationManager.operations;
    if (operations.length == 0) return;
    let logs = operations.map(m => {
      let types = [OBJECT_TYPE.label, OBJECT_TYPE.content];
      let objecttype = m.koobooComment.objecttype;
      if (objecttype && types.some(s => s == objecttype!.toLowerCase())) {
        objecttype = objecttype.toLowerCase();
      } else {
        objecttype = OBJECT_TYPE.dom;
      }

      let nameOrId = m.koobooComment.nameorid
        ? m.koobooComment.nameorid
        : m.koobooComment.bindingvalue;

      let log = new OperationLogItem();
      log.action = m.actionType;
      log.editorType = objecttype;
      log.koobooId = m.koobooId!;
      log.nameOrId = nameOrId!;
      log.objectType = m.koobooComment.objecttype!;
      log.value = cleanKoobooInfo(m.commit);
      log.fieldName = m.koobooComment.fieldname!;
      return log;
    });
    await updateOperation(logs);
    parent.location.reload();
  };
  return saveBtn;
}
