import { createItem, MenuItem } from "../basic";
import context from "@/common/context";
import {
  setGuid,
  markDirty,
  cleanKoobooInfo,
  isDynamicContent
} from "@/common/koobooInfo";
import { Operation } from "@/models/Operation";
import { KOOBOO_GUID, ACTION_TYPE } from "@/common/constants";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { TEXT } from "@/common/lang";
import { getEditComment } from "../utils";
import { isBody } from "@/dom/utils";

export function createCopyItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.COPY, MenuActions.copy);
  const update = () => {
    var visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (!args.parentKoobooId || !args.closeParent) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", e => {
    let args = context.lastSelectedDomEventArgs;
    if (!args.closeParent) return false;
    setGuid(args.closeParent);
    let startContent = args.closeParent.innerHTML;
    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    args.element.parentElement!.insertBefore(
      cloneElement,
      args.element.nextSibling
    );
    markDirty(args.closeParent);

    var endContent = args.closeParent!.innerHTML;
    let operation = new Operation(
      args.closeParent.getAttribute(KOOBOO_GUID)!,
      startContent,
      endContent,
      args.koobooComments[0],
      args.parentKoobooId,
      ACTION_TYPE.update,
      cleanKoobooInfo(args.closeParent!.innerHTML)
    );
    context.operationManager.add(operation);
  });

  return {
    el,
    update
  };
}
