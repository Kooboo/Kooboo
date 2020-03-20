import { getAllNode, getAllElement, previousNodes, nextNodes } from "../dom/utils";
import { KoobooComment } from "./KoobooComment";
import { KOOBOO_ID, KOOBOO_DIRTY, KOOBOO_GUID } from "../common/constants";
import { newGuid } from "./outsideInterfaces";
import { createDiv } from "@/dom/element";

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
  let innerHtml = el.innerHTML.trim();
  // if (innerHtml.startsWith("<!--#kooboo--source")) {
  //   innerHtml = innerHtml.substring(19);
  // }
  return innerHtml.indexOf("#kooboo--source") > -1;
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

export function getWarpContent(el: Node) {
  while (el && !KoobooComment.getAroundComments(el).length) {
    el = el.previousSibling ? (el.previousSibling as Node) : (el.parentElement as Node);
  }
  let comment = KoobooComment.getAroundComments(el)[0];
  let { nodes } = getWrapDom(el, comment.uid);
  nodes = nodes.map(m => m.cloneNode(true));
  let tempDiv = createDiv();
  tempDiv.append(...nodes);
  return tempDiv.innerHTML;
}

export function getWrapDom(el: Node, sourceOrUid: string) {
  let startNode: Node | undefined;
  let uid;
  let endNode: Node | undefined;
  let nodes: Node[] = [];

  for (const node of previousNodes(el, true, true)) {
    if (KoobooComment.isComment(node)) {
      let comment = new KoobooComment(node);
      if ((comment.source == sourceOrUid || comment.uid == sourceOrUid) && !KoobooComment.isEndComment(node)) {
        startNode = node;
        uid = comment.uid;
        break;
      }
    }
  }

  if (startNode) {
    for (const node of nextNodes(startNode, true, false)) {
      nodes.push(node);
      if (startNode!.nodeValue!.indexOf("attribute") > -1) {
        if (node instanceof Element) {
          endNode = node;
          break;
        }
      }

      if (KoobooComment.isComment(node)) {
        let comment = new KoobooComment(node);
        if (comment.uid == uid && KoobooComment.isEndComment(node)) {
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
  if (!includeSelf && el) el = el.parentElement!;

  while (el) {
    if (!el.hasAttribute(KOOBOO_DIRTY)) return el;
    if (KoobooComment.getAroundScopeComments(el)) return;
    el = el.parentElement!;
  }
}

export function getHasKoobooIdEl(el: HTMLElement, includeSelf = true) {
  if (!includeSelf && el) el = el.parentElement!;

  while (el) {
    if (el.hasAttribute(KOOBOO_ID)) return el;
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
