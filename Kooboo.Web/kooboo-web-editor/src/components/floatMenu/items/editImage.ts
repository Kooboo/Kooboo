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
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) setVisiable(false);
    if (getRepeatAttribute(args.koobooComments)) setVisiable(false);
    if (!getViewComment(args.koobooComments)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let el = args.element as HTMLImageElement;
    if (args.cleanElement) {
      updateDomImage(el, args.cleanElement, args.cleanKoobooId!, getViewComment(args.koobooComments)!);
    } else {
      updateAttributeImage(el, args.koobooId!, getEditComment(args.koobooComments)!);
    }
  });

  return { el, update };
}
