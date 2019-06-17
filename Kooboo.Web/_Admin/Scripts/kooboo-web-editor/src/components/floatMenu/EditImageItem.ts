import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import context from "../../context";
import { containDynamicContent } from "../../common/dom";

export class EditImageItem extends BaseItem {
  text: string = TEXT.TO_IMAGE;
  type: MenuActions = MenuActions.editImage;
  canShow(): boolean {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return false;
    if (args.element.tagName.toLowerCase() == "body") return false;
    if (!args.editableComment) return false;
    return !containDynamicContent(args.element);
  }
}
