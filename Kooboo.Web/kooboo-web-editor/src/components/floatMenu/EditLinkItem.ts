import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import context from "../../context";
import { containDynamicContent } from "../../common/dom";
import { pickLink } from "../../common/koobooInterfaces";
import { Operation } from "../../models/Operation";
import { KOOBOO_GUID, ACTION_TYPE } from "../../constants";
import { setGuid, cleanKoobooInfo } from "../../common/koobooInfo";

export class EditLinkItem extends BaseItem {
  text: string = TEXT.EDIT_LINK;
  type: MenuActions = MenuActions.editLink;
  canShow(): boolean {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return false;
    if (args.element.tagName.toLowerCase() != "a") return false;
    if (!args.parentKoobooId || !args.koobooId) return false;
    if (!args.editableComment) return false;
    return !containDynamicContent(args.element);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    if (!args || !args.closeParent) return false;
    setGuid(args.closeParent);
    let startContent = args.closeParent.innerHTML;
    let href = args.element.getAttribute("href")!;
    pickLink(url => {
      args!.element.setAttribute("href", url);
      let operation = new Operation(
        args!.closeParent!.getAttribute(KOOBOO_GUID)!,
        startContent,
        args!.closeParent!.innerHTML,
        args!.editableComment!,
        args!.parentKoobooId,
        ACTION_TYPE.update,
        cleanKoobooInfo(args!.closeParent!.innerHTML)
      );
      context.operationManager.add(operation);
    }, href);
  }
}
