import { KoobooComment } from "../models/koobooComment";
import { HOVER_BORDER_SKIP } from "../constants";

export function getKoobooComment(el: HTMLElement) {
  let node: Node | null = el as Node;
  let closeElement = el;

  while (true) {
    if (!node) break;

    if (
      node.nodeName == "#comment" &&
      node.nodeValue &&
      node.nodeValue.startsWith("#kooboo")
    ) {
      break;
    }

    if (node.previousSibling) {
      node = node.previousSibling;
      continue;
    }

    if (node.parentElement) {
      if (!closeElement.attributes.getNamedItem("kooboo-id"))
        closeElement = node.parentElement;
      node = node.parentNode;
    }
  }

  var koobooId = closeElement ? closeElement.getAttribute("kooboo-id") : null;
  var comment = node ? node.nodeValue : null;

  return {
    koobooComment: new KoobooComment(comment),
    closeEl: closeElement,
    koobooId: koobooId
  };
}

export function getMaxHeight(document: Document) {
  var body = document.body,
    html = document.documentElement;

  return Math.max(
    body.scrollHeight,
    body.offsetHeight,
    html.clientHeight,
    html.scrollHeight,
    html.offsetHeight
  );
}

export function isSkipHover(e: MouseEvent) {
  return ((e as any).path as Array<HTMLElement>).some(s => {
    if (s instanceof HTMLElement)
      return s.classList.contains(HOVER_BORDER_SKIP);

    return false;
  });
}
