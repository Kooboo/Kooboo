import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import {
  setGuid,
  cleanKoobooInfo,
  isDynamicContent
} from "@/common/koobooInfo";
import { pickLink } from "@/common/outsideInterfaces";
import { Operation } from "@/models/Operation";
import { KOOBOO_GUID, ACTION_TYPE } from "@/common/constants";
import { isLink } from "@/dom/utils";
import { getEditComment } from "../utils";
import { createLinkPicker } from "@/components/linkPicker";

export function createEditLinkItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_LINK, MenuActions.editLink);

  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;

    if (!isLink(args.element)) visiable = false;
    if (!args.closeParent || !args.parentKoobooId) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", e => {
    createLinkPicker();
    // let args = context.lastSelectedDomEventArgs;
    // if (!args.closeParent) return false;
    // setGuid(args.closeParent);
    // let startContent = args.closeParent.innerHTML;
    // let href = args.element.getAttribute("href")!;
    // pickLink(url => {
    //   args!.element.setAttribute("href", url);
    //   let operation = new Operation(
    //     args.closeParent!.getAttribute(KOOBOO_GUID)!,
    //     startContent,
    //     args.closeParent!.innerHTML,
    //     getEditComment(args.koobooComments)!,
    //     args!.parentKoobooId,
    //     ACTION_TYPE.update,
    //     cleanKoobooInfo(args!.closeParent!.innerHTML)
    //   );
    //   context.operationManager.add(operation);
    // }, href);
  });

  return { el, update };
}
