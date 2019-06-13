import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import context from "../../context";
import { OBJECT_TYPE, ACTION_TYPE, KOOBOO_GUID } from "../../constants";
import { containDynamicContent } from "../../common/dom";
import { markDirty, cleanKoobooInfo, setGuid } from "../../common/koobooInfo";
import { Operation } from "../../models/Operation";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";

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
    if (!args.koobooId) return false;
    if (!args.deletableComment) return false;

    return !containDynamicContent(args.element);
  }

  click() {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return;
    if (args.closeParent) {
      this.updateDom(args);
    } else if (args.element.parentElement) {
      this.deleteDom(args);
    }
  }
  private updateDom(args: SelectedDomEventArgs) {
    setGuid(args.closeParent!);
    let startContent = args.closeParent!.innerHTML;
    args.element.parentElement!.removeChild(args.element);
    markDirty(args.closeParent!);
    let endContent = args.closeParent!.innerHTML;
    let operation = new Operation(
      args.closeParent!.getAttribute(KOOBOO_GUID)!,
      startContent,
      endContent,
      args.editableComment!,
      args.parentKoobooId,
      ACTION_TYPE.update,
      cleanKoobooInfo(args.closeParent!.innerHTML)
    );
    context.operationManager.add(operation);
  }

  private deleteDom(args: SelectedDomEventArgs) {
    let parentElement = args.element.parentElement!;
    setGuid(parentElement);
    let startContent = parentElement.innerHTML;
    parentElement.removeChild(args.element);
    let endContent = parentElement.innerHTML;
    let operation = new Operation(
      parentElement.getAttribute(KOOBOO_GUID)!,
      startContent,
      endContent,
      args.deletableComment!,
      args.koobooId,
      ACTION_TYPE.delete,
      ""
    );
    context.operationManager.add(operation);
  }
}
