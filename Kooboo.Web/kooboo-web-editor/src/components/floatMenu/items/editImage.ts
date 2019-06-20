import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment } from "../utils";
import { isDynamicContent } from "@/common/koobooInfo";

export function createEditImageItem(): MenuItem {
  const { el, setVisiable } = createItem(
    TEXT.EDIT_IMAGE,
    MenuActions.editImage
  );
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) visiable = false;
    if (!args.closeParent || !args.parentKoobooId) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", e => {
    context.floatMenuClickEvent.emit(MenuActions.expand);
  });

  return { el, update };
}
