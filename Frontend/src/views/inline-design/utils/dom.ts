import {
  getChildNodes,
  getElements,
  isComment,
  isElement,
  replaceNode,
} from "@/utils/dom";
import { getCommentBinding } from "../binding";
import {
  KOOBOO_ID,
  K_ATTRIBUTE_PLACEHOLDER,
  K_BINDING,
  K_BINDING_ATTRIBUTE,
  K_BINDING_CONTENT,
  K_DIRTY,
  K_FORM_SCRIPT,
  K_REF,
  K_REMOVE,
} from "../constants";
import type { Position } from "../types";

export const getRect = (el?: HTMLElement) => {
  if (!el) return undefined;
  return el.getBoundingClientRect();
};

export const getScreenSize = (): Position => {
  return { x: window.innerWidth, y: window.innerHeight };
};

export function clearKoobooMark(content: string) {
  const container = document.createElement("div");
  container.innerHTML = content;
  const nodes = getChildNodes(container, true);

  for (const node of nodes) {
    if (isComment(node)) {
      const comment = node as Comment;
      const koobooComment = getCommentBinding(comment);
      if (koobooComment) {
        replaceNode(node);
      }
    }

    if (isElement(node)) {
      const el = node as Element;
      el.removeAttribute(K_REF);
      el.removeAttribute(K_DIRTY);
      el.removeAttribute(K_ATTRIBUTE_PLACEHOLDER);
      el.removeAttribute(KOOBOO_ID);
      el.removeAttribute(K_BINDING);
      el.removeAttribute(K_BINDING_CONTENT);
      el.removeAttribute(K_BINDING_ATTRIBUTE);

      if (el.hasAttribute(K_REMOVE) || el.hasAttribute(K_FORM_SCRIPT)) {
        el.remove();
      }
    }
  }

  return container.innerHTML;
}

export function getElementByRef(elements: HTMLElement[], ref: string) {
  return elements.find((f) => f.getAttribute(K_REF) == ref);
}
