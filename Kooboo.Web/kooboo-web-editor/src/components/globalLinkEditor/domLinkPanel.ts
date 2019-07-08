import { getAllElement, isLink } from "@/dom/utils";
import { getKoobooInfo, setGuid, clearKoobooInfo } from "@/kooboo/utils";
import { getDelete, getMenu, getForm, getHtmlBlock } from "../floatMenu/utils";
import { createLinkItem } from "./utils";
import { createLinkPicker } from "../linkPicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";

export function createDomLinkPanel() {
  let contiainer = document.createElement("div");

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLElement && isLink(element)) {
      let { comments, closeParent, parentKoobooId } = getKoobooInfo(element);
      if (getMenu(comments)) continue;
      if (getForm(comments)) continue;
      if (getHtmlBlock(comments)) continue;
      let comment = getDelete(comments);
      if (!comment || !closeParent || !parentKoobooId) continue;
      let item = createLinkItem(element, async () => {
        let startContent = closeParent!.innerHTML;
        try {
          let newValue = await createLinkPicker(element.getAttribute("href")!);
          element.setAttribute("href", newValue);
          let guid = setGuid(closeParent!);
          let value = clearKoobooInfo(closeParent!.innerHTML);
          let unit = new InnerHtmlUnit(startContent);
          let log = DomLog.createUpdate(comment!.nameorid!, value, parentKoobooId!, comment!.objecttype!);
          let record = new operationRecord([unit], [log], guid);
          context.operationManager.add(record);
        } catch (error) {
          closeParent!.innerHTML = startContent;
        }
      });
      contiainer.appendChild(item);
    }
  }

  return contiainer;
}
