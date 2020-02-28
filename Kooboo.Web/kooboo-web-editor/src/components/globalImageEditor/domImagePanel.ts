import { getAllElement } from "@/dom/utils";
import { updateDomImage, getEditableComment } from "../floatMenu/utils";
import { setImagePreview } from "./utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getUnpollutedEl } from "@/kooboo/utils";
import { createDiv } from "@/dom/element";
import { createImagePreview } from "../common/imagePreview";

export function createDomImagePanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLImageElement) {
      let aroundComments = KoobooComment.getAroundComments(element);
      let comments = KoobooComment.getComments(element);
      if (aroundComments.find(f => f.getValue("attribute") == "src")) continue;
      if (!getEditableComment(comments)) continue;
      if (!getUnpollutedEl(element)) continue;
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
