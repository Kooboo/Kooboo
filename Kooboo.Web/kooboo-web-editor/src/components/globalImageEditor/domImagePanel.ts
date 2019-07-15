import { getAllElement } from "@/dom/utils";
import { createImagePreview } from "@/components/common/imagePreview";
import { getViewComment, updateDomImage, updateAttributeImage, getAttributeComment } from "../floatMenu/utils";
import { setImagePreview } from "./utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { KOOBOO_ID } from "@/common/constants";
import { getCleanParent, isDirty } from "@/kooboo/utils";
import createDiv from "@/dom/div";

export function createDomImagePanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let comments = KoobooComment.getComments(element);
      let koobooId = element.getAttribute(KOOBOO_ID);
      if (getAttributeComment(comments)) continue;
      let comment = getViewComment(comments);
      if (!koobooId) continue;
      let { imagePreview, setImage } = createImagePreview(false, () => (element.src = ""));
      setImagePreview(imagePreview, element);
      setImage(element.src);

      imagePreview.onclick = async () => {
        let src: string | undefined;
        let { koobooId: parentKoobooId, parent } = getCleanParent(element);
        if (isDirty(element) && parent) {
          src = await updateDomImage(element, parent, parentKoobooId!, comment!);
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
