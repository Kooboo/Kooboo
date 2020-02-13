import { getAllNode, getAllElement, previousNodes, nextNodes } from "../dom/utils";
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
  while (el) {
    if (el.hasAttribute(KOOBOO_ID) || KoobooComment.getComments(el).length > 0) return el;
    el = el.parentElement!;
  }
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
  return el.innerHTML.indexOf("#kooboo") > -1;
}

export function getPageId() {
  for (const i of getAllNode(document)) {
    if (KoobooComment.isComment(i)) {
      let comment = new KoobooComment(i.nodeValue);
      if (comment.source == "page") {
        return comment.getValue("id")!;
      }
    }
  }
}

export function getWrapDom(el: Node, source: string) {
  let startNode: Node | undefined;
  let uid;
  let endNode: Node | undefined;
  let nodes: Node[] = [];

  for (const node of previousNodes(el, true, true)) {
    if (KoobooComment.isComment(node)) {
      let comment = new KoobooComment(node);
      if (comment.source == source && !KoobooComment.isEndComment(node)) {
        startNode = node;
        uid = comment.uid;
        break;
      }
    }
  }

  if (startNode) {
    for (const node of nextNodes(startNode, true, false)) {
      nodes.push(node);
      if (KoobooComment.isComment(node)) {
        let comment = new KoobooComment(node);
        if (comment.uid == uid) {
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

export function getUnpollutedEl(el: HTMLElement, includeSelf = true) {
  if (includeSelf && el) el = el.parentElement!;

  while (el) {
    if (isDynamicContent(el)) break;
    if (!el.hasAttribute(KOOBOO_DIRTY)) return el;
    el = el.parentElement!;
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
