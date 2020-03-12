import { getAllElement, isLink } from "@/dom/utils";
import { createLinkItem } from "./utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import { updateAttributeLink } from "../floatMenu/utils";

export function createContentLinkPanel() {
  let contiainer = createDiv();

  for (const element of getAllElement(document.body)) {
    if (element instanceof HTMLElement && isLink(element)) {
      let aroundComments = KoobooComment.getAroundComments(element);
      if (!aroundComments.find(f => f.getValue("attribute") == "href" && f.source != "none")) continue;
      let { item, setLabel } = createLinkItem(element, async () => {
        let newValue = await updateAttributeLink(element);
        if (newValue) setLabel(newValue);
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
