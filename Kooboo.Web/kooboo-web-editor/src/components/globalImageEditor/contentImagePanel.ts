import { getAllElement } from "@/dom/utils";
import { getKoobooInfo, setGuid } from "@/kooboo/utils";
import { getRepeatAttribute } from "../floatMenu/utils";
import { setImagePreview } from "./utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import context from "@/common/context";
import { operationRecord } from "@/operation/Record";
import { createImagePreview } from "../common/imagePreview";

export function createContentImagePanel() {
  let contiainer = document.createElement("div");

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let { comments } = getKoobooInfo(element);
      let comment = getRepeatAttribute(comments);
      if (!comment || !comment.fieldname || comment.attributename != "src") continue;
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
