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

export function isInMenu(comments: KoobooComment[]) {
  for (const i of comments) {
    if (i.objecttype && i.objecttype.toLowerCase() == OBJECT_TYPE.menu)
      return true;
  }
}

export function isInForm(comments: KoobooComment[]) {
  for (const i of comments) {
    if (i.objecttype && i.objecttype.toLowerCase() == OBJECT_TYPE.form)
      return true;
  }
}
