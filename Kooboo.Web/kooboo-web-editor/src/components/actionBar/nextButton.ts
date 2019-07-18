import { createButton } from "./button";
import nextIcon from "@/assets/icons/xiayibu.svg";
import nextEnableIcon from "@/assets/icons/xiayibu_enable.svg";
import { createNotice } from "./Notice";
import context from "@/common/context";
import { TEXT } from "@/common/lang";

export function createNextButton() {
  var nextBtn = createButton(nextIcon, TEXT.NEXT);
  nextBtn.onclick = () => context.operationManager.next();
  var nextNotice = createNotice();
  nextBtn.appendChild(nextNotice);
  context.operationEvent.addEventListener(e => {
    nextNotice.setCount(e.backupOperationCount);
    nextBtn.changeIcon(e.backupOperationCount > 0 ? nextEnableIcon : nextIcon);
  });
  return nextBtn;
}
