import { getAllElement } from "@/dom/utils";
import { getKoobooInfo, setGuid, clearKoobooInfo } from "@/kooboo/utils";
import { createImagePreview } from "@/components/common/imagePreview";
import { getEditComment } from "../floatMenu/utils";
import { setImagePreview } from "./utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";

export function createDomImagePanel() {
  let contiainer = document.createElement("div");

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let { comments, closeParent, parentKoobooId } = getKoobooInfo(element);
      let comment = getEditComment(comments);
      if (!comment || !closeParent || !parentKoobooId) continue;
      let { imagePreview, setImage } = createImagePreview(false, () => (element.src = ""));
      setImagePreview(imagePreview, element);
      setImage(element.src);
      imagePreview.onclick = () => {
        let startContent = closeParent!.innerHTML;
        pickImg(path => {
          element.src = path;
          setImage(path);
          let guid = setGuid(closeParent!);
          let value = clearKoobooInfo(closeParent!.innerHTML);
          let unit = new InnerHtmlUnit(startContent);
          let log = DomLog.createUpdate(comment!.nameorid!, value, parentKoobooId!, comment!.objecttype!);
          let record = new operationRecord([unit], [log], guid);
          context.operationManager.add(record);
        });
      };
      contiainer.appendChild(imagePreview);
    }
  }

  return contiainer;
}
