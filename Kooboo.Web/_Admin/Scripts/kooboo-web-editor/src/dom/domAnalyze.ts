import { KoobooComment } from "../models/koobooComment";

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
