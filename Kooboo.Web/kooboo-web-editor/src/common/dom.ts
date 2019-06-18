import { HOVER_BORDER_SKIP, OBJECT_TYPE } from "../constants";

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

export function* getAllElement(parentEl: HTMLElement, containSelf = false) {
  if (containSelf) yield parentEl;
  function* getChildren(el: Element): Iterable<Element> {
    for (let i = 0; i < el.children.length; i++) {
      const item = el.children[i];
      yield item;
      yield* getChildren(item);
    }
  }
  yield* getChildren(parentEl);
}

export function* getAllNode(parentNode: Node, containSelf = false) {
  if (containSelf) yield parentNode;
  function* getChildren(node: Node): Iterable<Node> {
    for (let i = 0; i < node.childNodes.length; i++) {
      const item = node.childNodes[i];
      yield item;
      yield* getChildren(item);
    }
  }
  yield* getChildren(parentNode);
}

export function containDynamicContent(el: HTMLElement) {
  for (const k in OBJECT_TYPE) {
    if (k == OBJECT_TYPE.url) continue;
    if (OBJECT_TYPE.hasOwnProperty(k)) {
      const i = OBJECT_TYPE[k as keyof typeof OBJECT_TYPE];
      if (el.innerHTML.toLowerCase().indexOf(`objecttype='${i}'`) > -1)
        return true;
    }
  }
  return false;
}

export function isBody(el: HTMLElement) {
  return el.tagName.toLowerCase() == "body";
}
