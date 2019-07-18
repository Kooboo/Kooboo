import { createButton } from "./button";
import preIcon from "@/assets/icons/shangyibu.svg";
import preEnableIcon from "@/assets/icons/shangyibu_enable.svg";
import { createNotice } from "./Notice";
import context from "@/common/context";
import { TEXT } from "@/common/lang";

export function createPreviousButton() {
  var preBtn = createButton(preIcon, TEXT.PREVIOUS);
  preBtn.onclick = () => context.operationManager.previous();
  var preNotice = createNotice();
  preBtn.appendChild(preNotice);
  context.operationEvent.addEventListener(e => {
    preNotice.setCount(e.operationCount);
    preBtn.changeIcon(e.operationCount > 0 ? preEnableIcon : preIcon);
  });
  return preBtn;
}
