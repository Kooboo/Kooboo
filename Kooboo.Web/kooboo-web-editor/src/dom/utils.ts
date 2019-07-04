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

export function getMaxWidth() {
  var body = document.body,
    html = document.documentElement;

  return Math.max(
    body.scrollWidth,
    body.offsetWidth,
    html.clientWidth,
    html.scrollWidth,
    html.offsetWidth
  );
}

export function isInEditorContainer(e: MouseEvent) {
  return ((e as any).path as Array<HTMLElement>).some(s => {
    if (s instanceof HTMLElement) return s.id == HOVER_BORDER_SKIP;

    return false;
  });
}

export function getEditorContainer() {
  return document.getElementById(HOVER_BORDER_SKIP)!;
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

export function canJump(el: HTMLElement) {
  let href = el.getAttribute("href");
  return el.tagName.toLowerCase() == "a" && href && !href.startsWith("#");
}

export function reload() {
  parent.location.reload();
}

export function* previousNodes(
  node: Node,
  containSelf = false,
  includeParent = false
) {
  if (containSelf) yield node;

  while (true) {
    if (node.previousSibling) {
      node = node.previousSibling;
    } else if (includeParent && node.parentNode) {
      node = node.parentNode;
    } else break;
    yield node;
  }
}

export function* nextNodes(
  node: Node,
  containSelf = false,
  includeParent = false
) {
  if (containSelf) yield node;

  while (true) {
    if (node.nextSibling) {
      node = node.nextSibling;
    } else if (includeParent && node.parentNode) {
      node = node.parentNode;
    } else break;
    yield node;
  }
}
