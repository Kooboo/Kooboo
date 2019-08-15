import { getAllNode, getAllElement, isBody, previousNodes, nextNodes } from "../dom/utils";
import { KoobooComment } from "./KoobooComment";
import { KOOBOO_ID, KOOBOO_DIRTY, KOOBOO_GUID, OBJECT_TYPE } from "../common/constants";
import { newGuid } from "./outsideInterfaces";

export function clearKoobooInfo(domString: string) {
  const exp = RegExp(
    `(data-mce-selected=".*?")|(data-mce-href=".*?")|(data-mce-style=".*?")|(${KOOBOO_ID}=".*?")|(${KOOBOO_DIRTY}(="")?)|(${KOOBOO_GUID}=".*?")|(<!--#?kooboo.*?-->)|(<!--empty-->)`,
    "g"
  );
  return domString.replace(exp, "");
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
    } else if (!el.parentElement || el.parentElement instanceof HTMLHtmlElement || (comment && !isSingleCommentWrap(comment))) {
      return { parent: null, koobooId: null };
    } else {
      el = el.parentElement;
      isParentFlag = true;
    }
  }
}

export function previousComment(node: Node) {
  for (const i of previousNodes(node)) {
    if (i instanceof Comment && KoobooComment.isComment(i)) {
      if (KoobooComment.isEndComment(i)) break;
      return i;
    }

    if (i instanceof HTMLElement) break;
  }
}

export function getRelatedRepeatComment(el: HTMLElement) {
  let comment = previousComment(el);
  if (!comment || !KoobooComment.isComment(comment) || !isSingleCommentWrap(comment)) return;
  let koobooComment = new KoobooComment(comment);
  if (koobooComment.end) return;
  for (const i of getAllNode(document.body)) {
    if (i instanceof Comment && KoobooComment.isComment(i) && !KoobooComment.isEndComment(i)) {
      let c = new KoobooComment(i);
      if (c.objecttype == OBJECT_TYPE.contentrepeater && c.nameorid == koobooComment.nameorid) {
        return c;
      }
    }
  }
}
