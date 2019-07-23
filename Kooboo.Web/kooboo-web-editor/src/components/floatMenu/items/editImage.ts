import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment, getViewComment, updateAttributeImage, updateDomImage, getAttributeComment } from "../utils";
import { isDynamicContent, getCleanParent, isDirty } from "@/kooboo/utils";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createEditImageItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_IMAGE, MenuActions.editImage);
  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (!isImg(args.element)) return setVisiable(false);
    if (getAttributeComment(comments)) return setVisiable(false);
    if (!getViewComment(comments)) return setVisiable(false);
    if (isDynamicContent(args.element)) return setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let el = args.element as HTMLImageElement;
    let { koobooId, parent } = getCleanParent(args.element);
    if (isDirty(args.element) && parent) {
      updateDomImage(el, parent, koobooId!, getViewComment(comments)!);
    } else {
      updateAttributeImage(el, args.koobooId!, getEditComment(comments)!);
    }
  });

  return { el, update };
}
