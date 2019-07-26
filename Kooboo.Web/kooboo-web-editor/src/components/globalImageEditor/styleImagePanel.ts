import { getAllElement, getBackgroundImage, clearBackgroundImage } from "@/dom/utils";
import { setGuid } from "@/kooboo/utils";
import { getViewComment } from "../floatMenu/utils";
import { KOOBOO_ID } from "@/common/constants";
import { setImagePreview } from "./utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import { createImagePreview } from "../common/imagePreview";
import { Background } from "@/dom/Background";

export function createStyleImagePanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (!(element instanceof HTMLElement)) continue;
    let { image, imageInBackground } = getBackgroundImage(element);
    if (!image || image == "none") continue;
    let koobooId = element.getAttribute(KOOBOO_ID);
    let comments = KoobooComment.getComments(element);
    let comment = getViewComment(comments);
    if (!comment || !koobooId) continue;
    let { imagePreview, setImage } = createImagePreview(false, () => clearBackgroundImage(element, imageInBackground));
    setImagePreview(imagePreview, element);
    if (image) setImage(image);
    imagePreview.onclick = () => {
      let startContent = element.getAttribute("style");
      pickImg(path => {
        if (imageInBackground) {
          let background = new Background(element.style.background!);
          background.image = `url('${path}')`;
          element.style.background = background.toString();
        } else {
          element.style.backgroundImage = `url('${path}')`;
        }
        setImage(path);
        let guid = setGuid(element);
        let unit = new AttributeUnit(startContent!, "style");
        let log: StyleLog;

        if (imageInBackground) {
          let value = element.style.background!.replace(/"/g, "'");
          log = StyleLog.createUpdate(comment!.nameorid!, comment!.objecttype!, value, "background", koobooId!);
        } else {
          let value = element.style.backgroundImage!.replace(/"/g, "'");
          log = StyleLog.createUpdate(comment!.nameorid!, comment!.objecttype!, value, "background-image", koobooId!);
        }

        let record = new operationRecord([unit], [log], guid);
        context.operationManager.add(record);
      });
    };
    contiainer.appendChild(imagePreview);
  }

  return contiainer;
}
