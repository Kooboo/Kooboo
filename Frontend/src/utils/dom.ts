import { computed, ref } from "vue";
import { getProperty, setProperty } from "./lang";

import type { AxiosResponse } from "axios";
import type { DomValueWrapper } from "@/global/types";
import { getPriority } from "@/global/style";

export const on = function (
  element: HTMLElement | Document | Window,
  event: string,
  handler: EventListenerOrEventListenerObject,
  useCapture = false
): void {
  if (element && event && handler) {
    element?.addEventListener(event, handler, useCapture);
  }
};

export const off = function (
  element: HTMLElement | Document | Window,
  event: string,
  handler: EventListenerOrEventListenerObject
) {
  if (element && event && handler) {
    element.removeEventListener(event, handler);
  }
};

export const download = function (blob: Blob, fileName: string): void {
  const URL = window.URL || window.webkitURL;
  const downloadUrl = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = downloadUrl;
  a.download = fileName;
  document.body.appendChild(a);
  a.click();
  setTimeout(() => {
    URL.revokeObjectURL(downloadUrl);
  }, 100); // cleanup
};

export const downloadFromResponse = function (
  response: AxiosResponse<Blob, unknown>
): void {
  const fileName = response.headers["content-disposition"].substring(
    "attachment;filename=".length
  );
  download(response.data, fileName);
};

export function domParse(html: string) {
  const domParser = new DOMParser();
  return domParser.parseFromString(html, "text/html");
}

export function domStringify(dom: Document) {
  let result = "";
  if (dom.doctype) {
    result += new XMLSerializer().serializeToString(dom.doctype);
  }
  result += dom.documentElement.outerHTML;
  return result;
}

export function getElementByTags(doc: Document, tags: string[]) {
  return getElements(doc).filter((i) => tags.some((f) => isTag(i, f)));
}

export function getElements(doc: Document) {
  const result: HTMLElement[] = [];
  const iterator = doc.createNodeIterator(doc.documentElement);
  let node = iterator.nextNode();

  while (node) {
    if (isElement(node)) result.push(node as HTMLElement);
    node = iterator.nextNode();
  }

  return result;
}

export function getComments(doc: Document) {
  const result: Comment[] = [];
  const iterator = doc.createNodeIterator(doc.documentElement);
  let node = iterator.nextNode();

  while (node) {
    if (isComment(node)) result.push(node as Comment);
    node = iterator.nextNode();
  }

  return result;
}

export function getChildNodes(
  el: Element,
  recursive = false,
  includeSelf = false
) {
  const result: Node[] = [];

  if (includeSelf) result.push(el);

  for (let i = 0; i < el.childNodes.length; i++) {
    const node = el.childNodes.item(i)!;
    result.push(node);

    if (recursive && isElement(node)) {
      result.push(...getChildNodes(node as Element, recursive, false));
    }
  }

  return result;
}

export function getChildElements(
  el: Element,
  recursive = false,
  includeSelf = false
) {
  return getChildNodes(el, recursive, includeSelf).filter((f) =>
    isElement(f)
  ) as HTMLElement[];
}

export function isTag(node: Node | null, name: string) {
  const el = node as Element;
  return el?.tagName?.toLowerCase() === name;
}

export function getElementParents(el: HTMLElement, includeSelf = false) {
  const list: HTMLElement[] = [];
  if (includeSelf) list.push(el);
  let parent = el.parentElement;

  while (parent) {
    list.push(parent);
    parent = parent.parentElement;
  }

  return list;
}

export function replaceNode(node: Node, nodes: Node[] | undefined = undefined) {
  if (nodes) {
    for (const i of nodes) {
      node.parentElement?.insertBefore(i, node);
    }
  }

  node.parentElement?.removeChild(node);
}

export function isElement(node: Node) {
  return "nodeType" in node && node.nodeType === 1;
}

export function isText(node: Node) {
  return "nodeType" in node && node.nodeType === 3;
}

export function isComment(node: Node) {
  return "nodeType" in node && node.nodeType === 8;
}

export function stringToDom(content: string) {
  const container = document.createElement("div");
  container.innerHTML = content;
  return container.firstChild;
}

export function stringToDoms(content: string) {
  const container = document.createElement("div");
  container.innerHTML = content;
  return getChildNodes(container);
}

export function useElement(el: Element, ...props: string[]) {
  const origin = getProperty(el, ...props);
  const state = ref(origin);

  const computedValue: any = computed({
    get() {
      return state.value;
    },
    set(value: any) {
      setProperty(el, value, ...props);
      state.value = value;
    },
  });

  const result = computedValue as DomValueWrapper;
  result.origin = origin;
  return result;
}

export function useAttribute(el: Element, name: string) {
  const origin = el.getAttribute(name);
  const state = ref(origin);

  const computedValue: any = computed({
    get() {
      return state.value;
    },
    set(value: any) {
      el.setAttribute(name, value);
      state.value = value;
    },
  });

  const result = computedValue as DomValueWrapper;
  result.origin = origin;
  result.name = name;
  return result;
}

export function useStyle(el: HTMLElement, name: string) {
  const origin = el.style.getPropertyValue(name);
  const state = ref(origin);
  const priority = getPriority(el, name);

  const computedValue: any = computed({
    get() {
      return state.value;
    },
    set(value: any) {
      el.style.setProperty(name, value, priority);
      state.value = value;
    },
  });

  const result = computedValue as DomValueWrapper;
  result.origin = origin;
  result.name = name;
  return result;
}

export function highLight(str: string, key: string) {
  if (!key) {
    return str;
  }

  const keyRule = key.trim().replace(/\s+/gi, "|");
  const reg = new RegExp(keyRule, "ig");
  return str.replace(reg, (val: any) => {
    return `<span style="color:rgba(34, 150, 243, 1)">${val}</span>`;
  });
}

export function generateCssText(rules: Record<string, string>) {
  const el = document.createElement("div");
  for (const key in rules) {
    if (Object.prototype.hasOwnProperty.call(rules, key)) {
      el.style.setProperty(key, rules[key]);
    }
  }
  return el.getAttribute("style");
}
