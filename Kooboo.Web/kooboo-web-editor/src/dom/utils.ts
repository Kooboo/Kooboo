import { HOVER_BORDER_SKIP, BACKGROUND_IMAGE_START, BACKGROUND_IMAGE_END } from "../common/constants";

export function getScrollTop() {
  return window.pageYOffset || document.documentElement.scrollTop;
}
export function getScrollLeft() {
  return window.pageXOffset || document.documentElement.scrollLeft;
}
export function getMaxHeight() {
  var body = document.body,
    html = document.documentElement;

  return Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
}

export function getMaxWidth() {
  var body = document.body,
    html = document.documentElement;

  return Math.max(body.scrollWidth, body.offsetWidth, html.clientWidth, html.scrollWidth, html.offsetWidth);
}

export function* getAllElement(parentEl: HTMLElement, containSelf = false) {
  if (containSelf) yield parentEl;

  function* getChildren(el: Element): Iterable<Element> {
    if (el instanceof HTMLElement && el.id == HOVER_BORDER_SKIP) return;

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
    if (node instanceof HTMLElement && node.id == HOVER_BORDER_SKIP) return;

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

export function isTextArea(el: Element) {
  return el instanceof HTMLTextAreaElement;
}

export function isForm(el: Element) {
  return el instanceof HTMLFormElement;
}

export function canJump(el: HTMLElement) {
  const prefixs = ["mailto:", "tel:", "sms:", "market", "geopoint:"];
  let href = el.getAttribute("href");
  if (!href) return false;
  href = href.trim().toLowerCase();
  for (const prefix of prefixs) {
    if (href!.trim().startsWith(prefix)) return false;
  }
  if (href.startsWith("#")) return false;
  if (href.startsWith("http") && !href.startsWith(window.location.origin.toLowerCase())) return false;
  return true;
}
export function reload() {
  window.top.location.reload();
  // parent.location.reload();
}

export function* previousNodes(node: Node, containSelf = false, includeParent = false) {
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

export function* nextNodes(node: Node, containSelf = false, includeParent = false) {
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

export function getParentElements(el: HTMLElement, includeSelf: boolean = true) {
  let elements: HTMLElement[] = [];
  if (includeSelf) elements.push(el);
  while (el) {
    el = el.parentElement!;
    if (el) elements.push(el);
  }
  return elements;
}

export function isInTable(el: HTMLElement) {
  var reExcept = /^(thead|tbody|tfoot|th|tr|td)$/i;
  return reExcept.test(el.tagName);
}

export function getImportant(el: HTMLElement, prop: string, cssRules: { cssRule: CSSStyleRule }[]) {
  let cssImportant = cssRules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority(prop));
  return cssImportant ? "important" : el.style.getPropertyPriority(prop);
}

export function shareStyle(container: HTMLElement) {
  let keywords = ["/*! Pickr", "/* kb_web_editor */"];
  for (let i = 0; i < document.styleSheets.length; i++) {
    let style = document.styleSheets.item(i)!;
    if (!(style.ownerNode instanceof HTMLElement)) return;
    if (keywords.some(s => (style.ownerNode as HTMLElement).innerHTML.startsWith(s))) {
      container.appendChild(style.ownerNode.cloneNode(true));
    }
  }
}

export function clearCssImageWarp(src: string) {
  if (!src) return "";
  src = src.trim().toLocaleLowerCase();
  if (BACKGROUND_IMAGE_START.test(src)) src = src.replace(BACKGROUND_IMAGE_START, "");
  if (BACKGROUND_IMAGE_END.test(src)) src = src.replace(BACKGROUND_IMAGE_END, "");
  return src;
}
