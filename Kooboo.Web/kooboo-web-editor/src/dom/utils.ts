import { HOVER_BORDER_SKIP, OBJECT_TYPE } from "../common/constants";

export function getMaxHeight() {
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

export function isInEditorContainer(e: MouseEvent) {
  return ((e as any).path as Array<HTMLElement>).some(s => {
    if (s instanceof HTMLElement) return s.id == HOVER_BORDER_SKIP;

    return false;
  });
}

export function getEditorContainer(doc: Document) {
  return doc.getElementById(HOVER_BORDER_SKIP)!;
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

export function isBody(el: HTMLElement) {
  return el.tagName.toLowerCase() == "body";
}

export function isImg(el: HTMLElement) {
  return el.tagName.toLowerCase() == "img";
}

export function isLink(el: HTMLElement) {
  return el.tagName.toLowerCase() == "a";
}
