import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment, getViewComment, updateAttributeImage, updateDomImage, getRepeatAttribute } from "../utils";
import { isDynamicContent } from "@/kooboo/utils";

export function createEditImageItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_IMAGE, MenuActions.editImage);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) visiable = false;
    if (getRepeatAttribute(args.koobooComments)) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let el = args.element as HTMLImageElement;
    if (args.closeParent) {
      updateDomImage(el, args.closeParent, args.parentKoobooId!, getViewComment(args.koobooComments)!);
    } else {
      updateAttributeImage(el, args.koobooId!, getEditComment(args.koobooComments)!);
    }
  });

  return { el, update };
}
