import { getAllElement } from "@/dom/utils";
import { setGuid } from "@/kooboo/utils";
import { setImagePreview } from "./utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import context from "@/common/context";
import { operationRecord } from "@/operation/Record";
import { createImagePreview } from "../common/imagePreview";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getAttributeComment } from "../floatMenu/utils";

export function createContentImagePanel() {
  let contiainer = document.createElement("div");

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let comments = KoobooComment.getComments(element);
      let comment = getAttributeComment(comments, "src");
      if (!comment || !comment.fieldname) continue;
      let { imagePreview, setImage } = createImagePreview(false, () => (element.src = ""));
      setImagePreview(imagePreview, element);
      setImage(element.src);
      imagePreview.onclick = () => {
        let startContent = element.src;
        pickImg(path => {
          element.src = path;
          setImage(path);
          let guid = setGuid(element);
          let value = element.src;
          let unit = new AttributeUnit(startContent, "src");
          let log = ContentLog.createUpdate(comment!.nameorid!, comment!.fieldname!, value);
          let record = new operationRecord([unit], [log], guid);
          context.operationManager.add(record);
        });
      };
      contiainer.appendChild(imagePreview);
    }
  }

  return contiainer;
}
