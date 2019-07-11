import { getAllNode, getAllElement, isBody, previousNodes, nextNodes, previousComment } from "../dom/utils";
import { KoobooComment } from "./KoobooComment";
import { KoobooId } from "./KoobooId";
import { KOOBOO_ID, KOOBOO_DIRTY, KOOBOO_GUID, OBJECT_TYPE } from "../common/constants";
import { newGuid } from "./outsideInterfaces";
import createDiv from "@/dom/div";

export function clearKoobooInfo(domString: string) {
  let el = createDiv();
  el.innerHTML = domString;
  let nodes = getAllNode(el);
  for (const i of nodes) {
    if (i instanceof HTMLElement) {
      if (i.hasAttribute(KOOBOO_ID)) i.attributes.removeNamedItem(KOOBOO_ID);
      if (i.hasAttribute(KOOBOO_DIRTY)) i.attributes.removeNamedItem(KOOBOO_DIRTY);
      if (i.hasAttribute(KOOBOO_GUID)) i.attributes.removeNamedItem(KOOBOO_GUID);
    }

    if (i.nodeType == Node.COMMENT_NODE && i.nodeValue && (i.nodeValue.startsWith(KOOBOO_GUID) || i.nodeValue == "empty")) {
      i.parentNode!.removeChild(i);
    }

    if (KoobooComment.isComment(i)) {
      i.parentNode!.removeChild(i);
    }
  }
  return el.innerHTML;
}

export function getCloseElement(el: HTMLElement) {
  let node = el as Node | null;
  let closeElement: HTMLElement | null = null;
  let parentElement: HTMLElement | null = null;

  while (true) {
    if (!node) break;

    if (node instanceof HTMLElement && parentElement != node.parentElement) {
      closeElement = node;
      if (node.hasAttribute(KOOBOO_ID)) break;
    }

    if (KoobooComment.isComment(node)) break;
    parentElement = node.parentElement;
    node = node.previousSibling ? node.previousSibling : node.parentElement;
  }

  return closeElement;
}

export function getMaxKoobooId(el: HTMLElement) {
  let id = el.getAttribute(KOOBOO_ID)!;
  var koobooId = new KoobooId(id);
  let nextTemp = el;

  while (true) {
    if (!nextTemp.nextElementSibling || !nextTemp.nextElementSibling.hasAttribute(KOOBOO_ID)) {
      break;
    }

    let nextId = nextTemp.nextElementSibling.getAttribute(KOOBOO_ID);
    let nextKoobooId = new KoobooId(nextId!);
    if (nextKoobooId.value > koobooId.value) koobooId = nextKoobooId;
    nextTemp = nextTemp.nextElementSibling as HTMLElement;
  }

  let previousTemp = el;

  while (true) {
    if (!previousTemp.previousElementSibling || !previousTemp.previousElementSibling.hasAttribute(KOOBOO_ID)) {
      break;
    }

    let previousId = previousTemp.previousElementSibling.getAttribute(KOOBOO_ID);
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

export function isDirty(el: HTMLElement) {
  return el.hasAttribute(KOOBOO_DIRTY);
}

export function setGuid(el: HTMLElement, relpase: string = "") {
  let guid = el.getAttribute(KOOBOO_GUID);
  if (relpase) guid = relpase;
  else if (!guid) guid = newGuid();
  el.setAttribute(KOOBOO_GUID, guid);
  return guid;
}

export function getGuidComment(guid: string) {
  return `<!--${KOOBOO_GUID} ${guid}-->`;
}

export function isDynamicContent(el: HTMLElement) {
  for (const k in OBJECT_TYPE) {
    if (k == OBJECT_TYPE.Url) continue;
    if (OBJECT_TYPE.hasOwnProperty(k)) {
      const i = OBJECT_TYPE[k as keyof typeof OBJECT_TYPE];
      if (el.innerHTML.indexOf(`objecttype='${i}'`) > -1) return true;
    }
  }

  return false;
}

export function getPageId() {
  let pageid!: string;

  for (const i of getAllNode(document)) {
    if (KoobooComment.isComment(i)) {
      let comment = new KoobooComment(i.nodeValue);
      if (comment.objecttype == OBJECT_TYPE.page) {
        pageid = comment.nameorid!;
        break;
      }
    }
  }

  return pageid;
}

export function getWrapDom(el: Node, objectType: string) {
  let startNode: Node | undefined;
  let boundary;
  let endNode: Node | undefined;
  let nodes: Node[] = [];

  for (const node of previousNodes(el, true, true)) {
    if (KoobooComment.isComment(node)) {
      let comment = new KoobooComment(node);
      if (comment.objecttype == objectType && !comment.end) {
        startNode = node;
        boundary = comment.boundary;
        break;
      }
    }
  }

  if (startNode) {
    let isSingle = isSingleCommentWrap(startNode);

    for (const node of nextNodes(startNode, true, false)) {
      nodes.push(node);

      if (isSingle && node instanceof HTMLElement) {
        endNode = node;
        break;
      }

      if (KoobooComment.isComment(node)) {
        let comment = new KoobooComment(node);

        if (comment.objecttype == objectType && comment.end && comment.boundary == boundary) {
          endNode = node;
          break;
        }
      }
    }
  }

  return {
    nodes,
    startNode,
    endNode
  };
}

export function isSingleCommentWrap(node: Node) {
  let singleCommentWrap = [OBJECT_TYPE.attribute, OBJECT_TYPE.content, OBJECT_TYPE.Label, OBJECT_TYPE.style, OBJECT_TYPE.Url];
  let comment = new KoobooComment(node);
  return singleCommentWrap.some(s => s == comment.objecttype);
}

export function getCleanParent(el: HTMLElement) {
  let isParentFlag = false;
  while (true) {
    let comment = previousComment(el);
    let dirty = el.hasAttribute(KOOBOO_DIRTY);
    let koobooId = el.getAttribute(KOOBOO_ID);
    if (!dirty && isParentFlag && koobooId) {
      return {
        parent: el,
        koobooId: koobooId
      };
    } else if (!el.parentElement || isBody(el.parentElement) || (comment && !isSingleCommentWrap(comment))) {
      return { parent: null, koobooId: null };
    } else {
      el = el.parentElement;
      isParentFlag = true;
    }
  }
}
