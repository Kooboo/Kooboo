import { getAllElement, isLink } from "@/dom/utils";
import { createLinkItem } from "./utils";
import { updateDomLink, getEditableComment, ElementAnalyze } from "../floatMenu/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";

export function createDomLinkPanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLElement && isLink(element)) {
      let { kooobooIdEl, fieldComment, operability } = ElementAnalyze(element);
      let aroundComments = KoobooComment.getAroundComments(element);
      if (aroundComments.find(f => f.getValue("attribute") == "href")) continue;
      if (!operability || (!kooobooIdEl && !fieldComment)) continue;

      let { item, setLabel } = createLinkItem(element, async () => {
        var url = await updateDomLink(element);
        if (url) setLabel(url);
      });
      contiainer.appendChild(item);
    }
  }

  if (contiainer.children.length > 0) {
    let el = contiainer.children.item(contiainer.children.length - 1) as HTMLElement;
    el.style.borderBottom = "none";
  }

  return contiainer;
}
