import { getAllElement, isLink } from "@/dom/utils";
import { getKoobooInfo } from "@/kooboo/utils";
import { createLinkItem } from "./utils";
import {
  getMenuComment,
  getFormComment,
  getHtmlBlockComment,
  getViewComment,
  getUrlComment,
  updateDomLink,
  updateUrlLink,
  updateAttributeLink,
  getRepeatAttribute
} from "../floatMenu/utils";

export function createDomLinkPanel() {
  let contiainer = document.createElement("div");

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLElement && isLink(element)) {
      let { comments, closeParent, parentKoobooId, koobooId } = getKoobooInfo(element);
      if (getMenuComment(comments)) continue;
      if (getFormComment(comments)) continue;
      if (getHtmlBlockComment(comments)) continue;
      if (getRepeatAttribute(comments)) continue;
      let urlComment = getUrlComment(comments)!;
      let viewComment = getViewComment(comments)!;

      let { item, setLabel } = createLinkItem(element, async () => {
        let url: string | undefined;
        if (closeParent) {
          url = await updateDomLink(closeParent, parentKoobooId!, element, viewComment);
        } else if (urlComment) {
          url = await updateUrlLink(element, koobooId!, urlComment, viewComment);
        } else {
          url = await updateAttributeLink(element, koobooId!, viewComment);
        }
        if (url != undefined) setLabel(url);
      });
      contiainer.appendChild(item);
    }
  }

  return contiainer;
}
