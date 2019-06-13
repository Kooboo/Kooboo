import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import context from "../../context";
import { OBJECT_TYPE } from "../../constants";
import { containDynamicContent } from "../../common/dom";

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
}
