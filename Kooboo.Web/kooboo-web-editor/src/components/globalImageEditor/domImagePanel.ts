import { getAllElement } from "@/dom/utils";
import { updateDomImage, ElementAnalyze } from "../floatMenu/utils";
import { setImagePreview } from "./utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import { createImagePreview } from "../common/imagePreview";

export function createDomImagePanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let { kooobooIdEl, fieldComment, operability } = ElementAnalyze(element);
      let aroundComments = KoobooComment.getAroundComments(element);
      if (aroundComments.find(f => f.getValue("attribute") == "src")) continue;
      if (!operability || (!kooobooIdEl && !fieldComment)) continue;
      let { imagePreview, setImage } = createImagePreview(false, () => (element.src = ""));
      setImagePreview(imagePreview, element);
      setImage(element.src);

      imagePreview.onclick = async () => {
        let src = await updateDomImage(element as HTMLImageElement);
        if (src) setImage(src);
      };
      contiainer.appendChild(imagePreview);
    }
  }

  return contiainer;
}
