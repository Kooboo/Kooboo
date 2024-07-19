import { isElement } from "@/utils/dom";

export function getFirstElement(doc: Document) {
  const range = getRange(doc);
  const node = range?.startContainer;
  if (!node) return null;
  return isElement(node) ? (node as Element) : node.parentElement;
}

export function wrapTag(doc: Document, tag: string) {
  //
}

const getRange = (doc: Document) => doc.getSelection()?.getRangeAt(0);
