import { BaseItem } from "./base";
import { TEXT } from "../../../lang";
import { MenuActions } from "../../../events/FloatMenuClickEvent";
import { containDynamicContent } from "../../../common/dom";
import context from "../../../context";
import { Operation } from "../../../models/Operation";
import { ACTION_TYPE } from "../../../constants";
import { cleanKoobooInfo, getMaxKoobooId } from "../../../common/koobooInfo";

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
    if (!args || !args.element.parentElement) return false;
    let startContent = args.closeParent!.innerHTML;
    var nextkoobooId = getMaxKoobooId(args.element);
    let cloneElement = args.element.cloneNode(true) as HTMLElement;
    cloneElement.setAttribute("kooboo-id", nextkoobooId);
    args.closeParent!.insertBefore(cloneElement, args.element);
    var endContent = args.closeParent!.innerHTML;
    let operation = new Operation(
      args.parentKoobooId!,
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
