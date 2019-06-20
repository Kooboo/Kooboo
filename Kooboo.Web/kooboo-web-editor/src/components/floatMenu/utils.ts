import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/models/KoobooComment";

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
