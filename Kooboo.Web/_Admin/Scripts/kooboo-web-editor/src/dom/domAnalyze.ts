import { KoobooComment } from "../models/KoobooComment";
import { HOVER_BORDER_SKIP } from "../constants";

export function getKoobooInfo(el: HTMLElement) {
  let node: Node | null = el as Node;
  let closeElement: HTMLElement | null = null;
  let koobooId: string | null = null;
  let comments: KoobooComment[] = [];
  while (true) {
    if (!node) break;
    if (!closeElement && node instanceof HTMLElement) {
      koobooId = node.getAttribute("kooboo-id");
      if (koobooId) closeElement = node;
    }

    if (
      node.nodeName == "#comment" &&
      node.nodeValue &&
      node.nodeValue.startsWith("#kooboo")
    ) {
      comments.push(new KoobooComment(node.nodeValue));
      node = node.parentElement;
      continue;
    }

    if (!node.previousSibling || node.previousSibling instanceof HTMLElement) {
      node = node.parentElement;
      continue;
    }

    node = node.previousSibling;
  }

  if (!closeElement) closeElement = el;
  return {
    comments,
    closeElement,
    koobooId
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

export function getAllElement(el: HTMLElement) {
  let elements: Element[] = [];

  function getChildren(el: Element) {
    for (let i = 0; i < el.children.length; i++) {
      const element = el.children[i];
      elements.push(element);
      getChildren(element);
    }
  }

  getChildren(el);
  return elements;
}
