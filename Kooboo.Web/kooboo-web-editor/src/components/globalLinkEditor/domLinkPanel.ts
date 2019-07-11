import { getAllElement, isLink } from "@/dom/utils";
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
  getAttributeComment
} from "../floatMenu/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { KOOBOO_ID } from "@/common/constants";
import { getCleanParent } from "@/kooboo/utils";
import createDiv from "@/dom/div";

export function createDomLinkPanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLElement && isLink(element)) {
      let comments = KoobooComment.getComments(element);
      let koobooId = element.getAttribute(KOOBOO_ID);
      if (!koobooId) continue;
      if (getMenuComment(comments)) continue;
      if (getFormComment(comments)) continue;
      if (getHtmlBlockComment(comments)) continue;
      if (getAttributeComment(comments)) continue;
      let urlComment = getUrlComment(comments)!;
      let viewComment = getViewComment(comments)!;

      let { item, setLabel } = createLinkItem(element, async () => {
        let url: string | undefined;
        let { koobooId: parentKoobooId, parent } = getCleanParent(element);
        if (parent) {
          url = await updateDomLink(parent, parentKoobooId!, element, viewComment);
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
