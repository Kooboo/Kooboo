import { getAllElement } from "@/dom/utils";
import { getKoobooInfo } from "@/kooboo/utils";
import { createImagePreview } from "@/components/common/imagePreview";
import { getViewComment, updateDomImage, updateAttributeImage, getRepeatAttribute } from "../floatMenu/utils";
import { setImagePreview } from "./utils";

export function createDomImagePanel() {
  let contiainer = document.createElement("div");

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let { comments, cleanElement, cleanKoobooId, koobooId } = getKoobooInfo(element);
      if (getRepeatAttribute(comments)) continue;
      let comment = getViewComment(comments);
      if (!koobooId) continue;
      let { imagePreview, setImage } = createImagePreview(false, () => (element.src = ""));
      setImagePreview(imagePreview, element);
      setImage(element.src);

      imagePreview.onclick = async () => {
        let src: string | undefined;
        if (cleanElement) {
          src = await updateDomImage(element, cleanElement, cleanKoobooId!, comment!);
        } else {
          src = await updateAttributeImage(element, koobooId!, comment!);
        }
        if (src != undefined) setImage(src);
      };
      contiainer.appendChild(imagePreview);
    }
  }

  return contiainer;
}
