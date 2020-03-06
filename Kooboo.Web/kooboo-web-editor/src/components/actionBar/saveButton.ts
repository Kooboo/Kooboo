import saveIcon from "@/assets/icons/baocun.svg";
import saveEnableIcon from "@/assets/icons/baocun_enable.svg";
import { createButton } from "./button";
import context from "@/common/context";
import updateOperation from "@/api/updateOperation";
import { reload } from "@/dom/utils";
import { TEXT } from "@/common/lang";
import { Log } from "@/operation/Log";

export function createSaveButton() {
  var saveBtn = createButton(saveIcon, TEXT.SAVE);
  context.operationEvent.addEventListener(e => {
    saveBtn.changeIcon(e.operationCount > 0 ? saveEnableIcon : saveIcon);
  });
  saveBtn.onclick = async () => {
    try {
      var logs: Log[] = [];

      for (const record of context.operationManager.previousRecords) {
        logs.push(...record.logs);
      }

      await updateOperation(logs);
      reload();
    } catch (error) {
      alert("save error!");
    }
  };
  return saveBtn;
}
