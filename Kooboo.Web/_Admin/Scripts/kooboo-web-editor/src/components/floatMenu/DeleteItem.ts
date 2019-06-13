import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import context from "../../context";
import { OBJECT_TYPE, ACTION_TYPE, KOOBOO_GUID } from "../../constants";
import { containDynamicContent } from "../../common/dom";
import { markDirty, cleanKoobooInfo, setGuid } from "../../common/koobooInfo";
import { Operation } from "../../models/Operation";

export class DeleteItem extends BaseItem {
  text: string = TEXT.DELETE;
  type: MenuActions = MenuActions.delete;
  canShow(): boolean {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return false;
    var comment = args.koobooComments[0];
    if (
      comment &&
      comment.objecttype &&
      comment.objecttype.toLowerCase() == OBJECT_TYPE.label
    ) {
      return true;
    }

    if (args.element.tagName.toLowerCase() == "body") return false;
    if (!args.parentKoobooId) return false;

    return !containDynamicContent(args.element);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return;
    if (args.closeParent) {
      setGuid(args.closeParent);
      let startContent = args.closeParent.innerHTML;
      args.element.parentElement!.removeChild(args.element);
      markDirty(args.closeParent);
      let endContent = args.closeParent.innerHTML;
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
      return;
    }
  }
}
