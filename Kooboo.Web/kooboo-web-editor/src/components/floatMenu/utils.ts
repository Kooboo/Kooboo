import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationManager } from "@/operation/Manager";
import { getAllNode } from "@/dom/utils";

export function getEditComment(comments: KoobooComment[]) {
  const editTypes = [OBJECT_TYPE.view, OBJECT_TYPE.page, OBJECT_TYPE.layout, OBJECT_TYPE.content, OBJECT_TYPE.label];

  for (const i of comments) {
    if (i.objecttype && editTypes.some(s => s == i.objecttype)) return i;
  }
}

export function getDelete(comments: KoobooComment[]) {
  const editTypes = [OBJECT_TYPE.view, OBJECT_TYPE.page, OBJECT_TYPE.layout];

  for (const i of comments) {
    if (i.objecttype && editTypes.some(s => s == i.objecttype)) return i;
  }
}

export function getRepeatAttribute(comments: KoobooComment[]) {
  let comment = comments[0];
  if (comment && comment.objecttype == OBJECT_TYPE.attribute) {
    return comment;
  }
}

function getObjectType(comments: KoobooComment[], type: string) {
  for (const i of comments) {
    if (i.objecttype && i.objecttype.toLowerCase() == type) return i;
  }
}

export function hasOperation(operationManager: operationManager) {
  return operationManager.previousRecords.length > 0 || operationManager.nextRecords.length > 0;
}

export const getMenu = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.menu);

export const getForm = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.form);

export const getHtmlBlock = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.htmlblock);

export const getRepeat = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.contentrepeater);

export function changeNameOrId(node: Node, guid: string) {
  if (KoobooComment.isKoobooComment(node)) {
    node.nodeValue = node.nodeValue!.replace(/--nameorid='.{36,50}?'/, `--nameorid='${guid}'`);
  }
  if (node instanceof HTMLElement) {
    for (const iterator of getAllNode(node)) {
      changeNameOrId(iterator, guid);
    }
  }
}
