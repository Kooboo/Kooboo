import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/models/KoobooComment";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";

export function getEditComment(comments: KoobooComment[]) {
  const editTypes = [
    OBJECT_TYPE.view,
    OBJECT_TYPE.page,
    OBJECT_TYPE.layout,
    OBJECT_TYPE.content,
    OBJECT_TYPE.label
  ];

  for (const i of comments) {
    if (i.objecttype && editTypes.some(s => s == i.objecttype!.toLowerCase()))
      return i;
  }
}

function isObjectType(comments: KoobooComment[], type: string) {
  for (const i of comments) {
    if (i.objecttype && i.objecttype.toLowerCase() == type) return true;
  }
}

export const isInMenu = (comments: KoobooComment[]) =>
  isObjectType(comments, OBJECT_TYPE.menu);

export const isInForm = (comments: KoobooComment[]) =>
  isObjectType(comments, OBJECT_TYPE.form);

export const isHtmlBlock = (comments: KoobooComment[]) =>
  isObjectType(comments, OBJECT_TYPE.menu);
