import { getAllNode, containDynamicContent, getAllElement } from "./dom";
import { KoobooComment } from "../models/KoobooComment";
import { KoobooId } from "../models/KoobooId";
import { KOOBOO_ID, KOOBOO_DIRTY, KOOBOO_GUID } from "../constants";

export function cleanKoobooInfo(domString: string) {
  let el = document.createElement("div");
  el.innerHTML = domString;
  let nodes = getAllNode(el);
  for (const i of nodes) {
    if (i instanceof HTMLElement) {
      if (i.hasAttribute(KOOBOO_ID)) i.attributes.removeNamedItem(KOOBOO_ID);

      if (i.hasAttribute(KOOBOO_DIRTY))
        i.attributes.removeNamedItem(KOOBOO_DIRTY);

      if (i.hasAttribute(KOOBOO_GUID))
        i.attributes.removeNamedItem(KOOBOO_GUID);
    }

    if (
      i instanceof Comment &&
      i.nodeValue &&
      i.nodeValue.startsWith("#kooboo")
    ) {
      i.parentNode!.removeChild(i);
    }
  }
  return el.innerHTML;
}

export function getKoobooInfo(el: HTMLElement) {
  let koobooId = el.getAttribute(KOOBOO_ID);
  let node = el as Node | null;
  let closeParent: HTMLElement | null = null;
  let parentKoobooId: string | null = null;
  let comments: KoobooComment[] = [];
  let parentLayerFlag = false;

  while (true) {
    if (!node) break;
    if (
      parentLayerFlag &&
      !closeParent &&
      comments.length == 0 &&
      node instanceof HTMLElement &&
      !node.hasAttribute(KOOBOO_DIRTY) &&
      !containDynamicContent(node)
    ) {
      parentKoobooId = node.getAttribute(KOOBOO_ID);
      if (parentKoobooId) closeParent = node;
    }

    if (
      node.nodeName == "#comment" &&
      node.nodeValue &&
      node.nodeValue.startsWith("#kooboo")
    ) {
      let comment = new KoobooComment(node.nodeValue);
      if (comment.objecttype != "Url") comments.push(comment);
      node = node.parentElement;
      continue;
    }

    if (!node.previousSibling || node.previousSibling instanceof HTMLElement) {
      node = node.parentElement;
      parentLayerFlag = true;
      continue;
    }

    node = node.previousSibling;
  }

  return {
    comments,
    koobooId,
    closeParent,
    parentKoobooId
  };
}

export function getCloseElement(el: HTMLElement) {
  let node = el as Node | null;
  let closeElement: HTMLElement | null = null;
  while (true) {
    if (!node) break;
    if (node instanceof HTMLElement) {
      closeElement = node;
      if (node.hasAttribute(KOOBOO_ID)) break;
    }

    if (
      node.nodeName == "#comment" &&
      node.nodeValue &&
      node.nodeValue.startsWith("#kooboo")
    ) {
      break;
    }

    node = node.previousSibling ? node.previousSibling : node.parentElement;
  }

  return closeElement;
}

export function getMaxKoobooId(el: HTMLElement) {
  let id = el.getAttribute(KOOBOO_ID)!;
  var koobooId = new KoobooId(id);
  let nextTemp = el;
  while (true) {
    if (
      !nextTemp.nextElementSibling ||
      !nextTemp.nextElementSibling.hasAttribute(KOOBOO_ID)
    ) {
      break;
    }

    let nextId = nextTemp.nextElementSibling.getAttribute(KOOBOO_ID);
    let nextKoobooId = new KoobooId(nextId!);
    if (nextKoobooId.value > koobooId.value) koobooId = nextKoobooId;
    nextTemp = nextTemp.nextElementSibling as HTMLElement;
  }

  let previousTemp = el;
  while (true) {
    if (
      !previousTemp.previousElementSibling ||
      !previousTemp.previousElementSibling.hasAttribute(KOOBOO_ID)
    ) {
      break;
    }

    let previousId = previousTemp.previousElementSibling.getAttribute(
      KOOBOO_ID
    );
    let previousKoobooId = new KoobooId(previousId!);
    if (previousKoobooId.value > koobooId.value) koobooId = previousKoobooId;
    previousTemp = previousTemp.previousElementSibling as HTMLElement;
  }

  return koobooId.next;
}

export function markDirty(el: HTMLElement, self: boolean = false) {
  for (const i of getAllElement(el, self)) {
    i.setAttribute(KOOBOO_DIRTY, "");
  }
}

export function setGuid(el: HTMLElement) {
  if (!el.hasAttribute(KOOBOO_GUID)) {
    el.setAttribute(KOOBOO_GUID, Math.random().toString());
  }
}
