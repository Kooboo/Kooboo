import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { containDynamicContent, getAllElement } from "../../common/dom";
import context from "../../context";
import { Operation } from "../../models/Operation";
import { ACTION_TYPE, KOOBOO_DIRTY, KOOBOO_GUID } from "../../constants";
import { cleanKoobooInfo } from "../../common/koobooInfo";

export class CopyItem extends BaseItem {
  text: string = TEXT.COPY;
  type: MenuActions = MenuActions.copy;
  canShow(): boolean {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return false;

    if (args.element.tagName.toLowerCase() == "body") return false;
    if (!args.parentKoobooId || !args.koobooId) return false;

    return !containDynamicContent(args.element);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    if (!args || !args.closeParent) return false;

    if (!args.closeParent.hasAttribute(KOOBOO_GUID)) {
      args.closeParent.setAttribute(KOOBOO_GUID, Math.random().toString());
    }

    let startContent = args.closeParent.innerHTML;
    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    args.closeParent.insertBefore(cloneElement, args.element);

    for (const i of getAllElement(args.closeParent)) {
      i.setAttribute(KOOBOO_DIRTY, "");
    }

    var endContent = args.closeParent!.innerHTML;
    let operation = new Operation(
      args.closeParent.getAttribute(KOOBOO_GUID)!,
      startContent,
      endContent,
      args.editComment!,
      args.parentKoobooId,
      ACTION_TYPE.update,
      cleanKoobooInfo(args.closeParent!.innerHTML)
    );
    context.operationManager.add(operation);
  }
}
