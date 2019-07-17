import { getAllElement, isLink } from "@/dom/utils";
import { setGuid } from "@/kooboo/utils";
import { createLinkItem } from "./utils";
import { createLinkPicker } from "../linkPicker";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getAttributeComment } from "../floatMenu/utils";
import { createDiv } from "@/dom/element";

export function createContentLinkPanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLElement && isLink(element)) {
      let comments = KoobooComment.getComments(element);
      let comment = getAttributeComment(comments, "href");
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

  if (contiainer.children.length > 0) {
    let el = contiainer.children.item(contiainer.children.length - 1) as HTMLElement;
    el.style.borderBottom = "none";
  }

  return contiainer;
}
