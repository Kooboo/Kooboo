import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/models/KoobooComment";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import { OperationManager } from "@/models/OperationManager";

export function getEditComment(comments: KoobooComment[]) {
  const editTypes = [
    OBJECT_TYPE.view,
    OBJECT_TYPE.page,
    OBJECT_TYPE.layout,
    OBJECT_TYPE.content,
    OBJECT_TYPE.label
  ];

  for (const i of comments) {
    if (i.objecttype && editTypes.some(s => s == i.objecttype)) return i;
  }
}

function getObjectType(comments: KoobooComment[], type: string) {
  for (const i of comments) {
    if (i.objecttype && i.objecttype.toLowerCase() == type) return i;
  }
}

export function hasOperation(operationManager: OperationManager) {
  return (
    operationManager.backupOperations.length > 0 ||
    operationManager.operations.length > 0
  );
}

export const getMenu = (comments: KoobooComment[]) =>
  getObjectType(comments, OBJECT_TYPE.menu);

export const getForm = (comments: KoobooComment[]) =>
  getObjectType(comments, OBJECT_TYPE.form);

export const getHtmlBlock = (comments: KoobooComment[]) =>
  getObjectType(comments, OBJECT_TYPE.htmlblock);

export const getRepeat = (comments: KoobooComment[]) =>
  getObjectType(comments, OBJECT_TYPE.contentrepeater);
