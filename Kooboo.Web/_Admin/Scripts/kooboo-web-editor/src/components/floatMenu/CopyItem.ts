import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { containDynamicContent, isBody } from "../../common/dom";
import context from "../../context";
import { Operation } from "../../models/Operation";
import { ACTION_TYPE, KOOBOO_GUID } from "../../constants";
import { cleanKoobooInfo, markDirty, setGuid } from "../../common/koobooInfo";

export class CopyItem extends BaseItem {
  text: string = TEXT.COPY;
  type: MenuActions = MenuActions.copy;
  canShow(): boolean {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return false;

    if (isBody(args.element)) return false;
    if (!args.parentKoobooId || !args.koobooId) return false;
    if (!args.editableComment) return false;
    return !containDynamicContent(args.element);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    if (!args || !args.closeParent) return false;
    setGuid(args.closeParent);
    let startContent = args.closeParent.innerHTML;
    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    args.closeParent.insertBefore(cloneElement, args.element.nextSibling);
    markDirty(args.closeParent);

    var endContent = args.closeParent!.innerHTML;
    let operation = new Operation(
      args.closeParent.getAttribute(KOOBOO_GUID)!,
      startContent,
      endContent,
      args.editableComment!,
      args.parentKoobooId,
      ACTION_TYPE.update,
      cleanKoobooInfo(args.closeParent!.innerHTML)
    );
    context.operationManager.add(operation);
  }
}
