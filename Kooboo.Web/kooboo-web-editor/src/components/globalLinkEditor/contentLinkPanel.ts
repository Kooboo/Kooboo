import { getAllElement, isLink } from "@/dom/utils";
import { getKoobooInfo, setGuid } from "@/kooboo/utils";
import { getRepeatAttribute } from "../floatMenu/utils";
import { createLinkItem } from "./utils";
import { createLinkPicker } from "../linkPicker";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";

export function createContentLinkPanel() {
  let contiainer = document.createElement("div");

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLElement && isLink(element)) {
      let { comments } = getKoobooInfo(element);
      let comment = getRepeatAttribute(comments);
      if (!comment || !comment.fieldname || comment.attributename != "href") continue;
      let { item, setLabel } = createLinkItem(element, async () => {
        let startContent = element.getAttribute("href")!;
        try {
          let newValue = await createLinkPicker(startContent);
          element.setAttribute("href", newValue);
          let guid = setGuid(element);
          let value = element.getAttribute("href")!;
          let unit = new AttributeUnit(startContent, "href");
          let log = ContentLog.createUpdate(comment!.nameorid!, comment!.fieldname!, value);
          let record = new operationRecord([unit], [log], guid);
          context.operationManager.add(record);
          setLabel(newValue);
        } catch (error) {
          element.setAttribute("href", startContent);
        }
      });
      contiainer.appendChild(item);
    }
  }

  return contiainer;
}
