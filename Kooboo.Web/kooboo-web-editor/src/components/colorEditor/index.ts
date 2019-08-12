import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getMatchedColorGroups } from "@/dom/style";
import { createDiv } from "@/dom/element";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { CssUnit } from "@/operation/recordUnits/CssUnit";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { setGuid } from "@/kooboo/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Log } from "@/operation/recordLogs/Log";
import { createColorEditItem } from "./colorEditItem";

export function createColorEditor(el: HTMLElement, comment: KoobooComment, koobooId: string) {
  var startStyle = el.getAttribute("style");
  var groups = getMatchedColorGroups(el);
  let container = createDiv();
  let logGetters: (() => Log[])[] = [];
  for (const i of groups) {
    let { item, getLogs } = createColorEditItem(i, el, comment, koobooId);
    logGetters.push(getLogs);
    container.appendChild(item);
  }

  const getUnits = () => [new AttributeUnit(startStyle || "", "style"), new CssUnit("", groups)];

  let { modal, close, setCancelHandler, setOkHandler } = createModal(TEXT.EDIT_COLOR, container, "600px");
  setCancelHandler(() => {
    getUnits().map(m => m.undo(el));
    close();
  });

  setOkHandler(() => {
    let guid = setGuid(el);
    let logs: Log[] = [];
    for (const i of logGetters) {
      logs.push(...i());
    }
    let operation = new operationRecord(getUnits(), logs, guid);
    context.operationManager.add(operation);
    close();
  });
  context.container.appendChild(modal);
}
