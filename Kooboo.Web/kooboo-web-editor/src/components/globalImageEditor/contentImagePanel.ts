import { getAllElement } from "@/dom/utils";
import { setGuid } from "@/kooboo/utils";
import { setImagePreview } from "./utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import context from "@/common/context";
import { operationRecord } from "@/operation/Record";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import { createImagePreview } from "../common/imagePreview";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";
import { ElementAnalyze } from "../floatMenu/utils";

export function createContentImagePanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let { kooobooIdEl, fieldComment, operability } = ElementAnalyze(element);
      let aroundComments = KoobooComment.getAroundComments(element);
      if (!aroundComments.find(f => f.getValue("attribute") == "src" && f.source != "none")) continue;
      if (!operability || (!kooobooIdEl && !fieldComment)) continue;
      let { imagePreview, setImage } = createImagePreview(false, () => (element.src = ""));
      setImagePreview(imagePreview, element);
      setImage(element.src);
      imagePreview.onclick = () => {
        let comments = KoobooComment.getAroundComments(element);
        let comment = comments.find(f => f.getValue("attribute") == "src")!;
        let img = element as HTMLImageElement;
        let startContent = img.getAttribute("src")!;
        pickImg(path => {
          img.src = path;
          setImage(path);
          let guid = setGuid(img);
          let value = img.getAttribute("src")!;
          let unit = new AttributeUnit(startContent, "src");
          let log = [...comment.infos, kvInfo.value(value)];
          let record = new operationRecord([unit], [new Log(log)], guid);
          context.operationManager.add(record);
        });
      };
      contiainer.appendChild(imagePreview);
    }
  }

  return contiainer;
}
