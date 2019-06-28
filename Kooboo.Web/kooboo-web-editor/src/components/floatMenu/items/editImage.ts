import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment } from "../utils";
import {
  isDynamicContent,
  setGuid,
  markDirty,
  cleanKoobooInfo
} from "@/common/koobooInfo";
import { createImagePicker } from "@/components/imagePicker";
import { Operation } from "@/models/Operation";
import { KOOBOO_GUID, ACTION_TYPE, EDITOR_TYPE } from "@/common/constants";
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

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    if (!args.closeParent) return false;
    setGuid(args.closeParent);
    let startContent = args.closeParent.innerHTML;
    try {
      await createImagePicker(args.element as HTMLImageElement);
      markDirty(args.closeParent);
      var endContent = args.closeParent!.innerHTML;
      let operation = new Operation(
        args.closeParent.getAttribute(KOOBOO_GUID)!,
        startContent,
        endContent,
        getEditComment(args.koobooComments)!,
        args.parentKoobooId,
        ACTION_TYPE.update,
        cleanKoobooInfo(args.closeParent!.innerHTML),
        EDITOR_TYPE.dom
      );
      context.operationManager.add(operation);
    } catch (error) {
      args.closeParent.innerHTML = startContent;
    }
  });

  return { el, update };
}
