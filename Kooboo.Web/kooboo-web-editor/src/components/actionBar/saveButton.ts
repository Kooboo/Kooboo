import saveIcon from "../../assets/icons/baocun.svg";
import saveEnableIcon from "../../assets/icons/baocun_enable.svg";
import { createButton } from "./button";
import context from "../../common/context";
import updateOperation from "../../api/updateOperation";
import { reload } from "@/dom/utils";
import { Log } from "@/operation/recordLogs/Log";
import { getMargedLogs } from "@/operation/untils";

export function createSaveButton(document: Document) {
  var saveBtn = createButton(document, saveIcon);
  context.operationEvent.addEventListener(e => {
    saveBtn.changeIcon(e.operationCount > 0 ? saveEnableIcon : saveIcon);
  });
  saveBtn.onclick = async () => {
    let logs: Log[] = [];
    context.operationManager.previousRecords.forEach(i => {
      logs.push(...i.logs);
    });
    logs = getMargedLogs(logs);
    if (logs.length == 0) return;
    await updateOperation(logs.map(m => m.getCommitObject()));
    reload();
  };
  return saveBtn;
}
