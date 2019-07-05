import saveIcon from "@/assets/icons/baocun.svg";
import saveEnableIcon from "@/assets/icons/baocun_enable.svg";
import { createButton } from "./button";
import context from "@/common/context";
import updateOperation from "@/api/updateOperation";
import { reload } from "@/dom/utils";
import { Log } from "@/operation/recordLogs/Log";
import { TEXT } from "@/common/lang";

export function createSaveButton(document: Document) {
  var saveBtn = createButton(document, saveIcon, TEXT.SAVE);
  context.operationEvent.addEventListener(e => {
    saveBtn.changeIcon(e.operationCount > 0 ? saveEnableIcon : saveIcon);
  });
  saveBtn.onclick = async () => {
    let logs: Log[] = [];

    for (const iterator of context.operationManager.previousRecords) {
      logs.push(...iterator.logs);
    }
    if (logs.length == 0) return;
    try {
      await updateOperation(logs);
      reload();
    } catch (error) {
      alert("save error!");
    }
  };
  return saveBtn;
}
