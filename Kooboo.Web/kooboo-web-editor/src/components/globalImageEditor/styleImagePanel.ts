import { getAllElement } from "@/dom/utils";
import { setGuid } from "@/kooboo/utils";
import { getViewComment } from "../floatMenu/utils";
import { KOOBOO_ID } from "@/common/constants";
import { createImagePreview } from "../common/imagePreview";
import { setImagePreview } from "./utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { KoobooComment } from "@/kooboo/KoobooComment";
import createDiv from "@/dom/div";

export function createStyleImagePanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    let style = getComputedStyle(element);
    if (style.backgroundImage != "none" && element instanceof HTMLElement) {
      let koobooId = element.getAttribute(KOOBOO_ID);
      let comments = KoobooComment.getComments(element);
      let comment = getViewComment(comments);
      if (!comment || !koobooId) continue;
      let { imagePreview, setImage } = createImagePreview(false, () => (element.style.backgroundImage = "none"));
      setImagePreview(imagePreview, element);
      setImage(style.backgroundImage!);
      imagePreview.onclick = () => {
        let startContent = element.getAttribute("style");
        pickImg(path => {
          element.style.backgroundImage = `url(${path})`;
          setImage(path);
          let guid = setGuid(element);
          let value = element.style.backgroundImage.replace(/"/g, "'");
          let unit = new AttributeUnit(startContent!, "style");
          let log = StyleLog.createUpdate(comment!.nameorid!, comment!.objecttype!, value, "background-image", koobooId!);
          let record = new operationRecord([unit], [log], guid);
          context.operationManager.add(record);
        });
      };
      contiainer.appendChild(imagePreview);
    }
  }

  return contiainer;
}
