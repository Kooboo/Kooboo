import saveIcon from "../../assets/icons/baocun.svg";
import saveEnableIcon from "../../assets/icons/baocun_enable.svg";
import { createButton } from "./button";
import context from "../../common/context";
import { OperationLogItem } from "../../models/OperationLog";
import updateOperation from "../../api/updateOperation";
import { OBJECT_TYPE } from "../../common/constants";
import { cleanKoobooInfo } from "../../common/koobooInfo";
import { reload } from "@/dom/utils";

export function createSaveButton(document: Document) {
  var saveBtn = createButton(document, saveIcon);
  context.operationEvent.addEventListener(e => {
    saveBtn.changeIcon(e.operationCount > 0 ? saveEnableIcon : saveIcon);
  });
  saveBtn.onclick = async () => {
    let logs = context.operationManager.operationLogs;
    if (logs.length == 0) return;
    await updateOperation(logs);
    reload();
  };
  return saveBtn;
}
