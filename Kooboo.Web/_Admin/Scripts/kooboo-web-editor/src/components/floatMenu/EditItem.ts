import { TEXT } from "../../lang";
import { BaseItem } from "./BaseItem";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { OBJECT_TYPE } from "../../constants";
import { containDynamicContent } from "../../common/dom";
import context from "../../context";

export class EditItem extends BaseItem {
  type: MenuActions = MenuActions.edit;
  text: string = TEXT.EDIT;

  canShow(): boolean {
    let args = context.lastSelectedDomEventArgs;
    if (!args || args.koobooComments.length == 0 || !args.element) {
      return false;
    }
    let comment = args.koobooComments[0];
    let el = args.element;
    let id = args.koobooId;

    if (
      !id &&
      comment.objecttype &&
      comment.objecttype.toLowerCase() != OBJECT_TYPE.label
    ) {
      return false;
    }

    if (
      comment.objecttype &&
      comment.objecttype.toLowerCase() == OBJECT_TYPE.content &&
      !comment.nameorid
    ) {
      return false;
    }

    if (
      comment.objecttype &&
      (comment.objecttype.toLowerCase() == OBJECT_TYPE.menu ||
        comment.objecttype.toLowerCase() == OBJECT_TYPE.form)
    ) {
      return false;
    }

    var reExcept = /^img|button|input|textarea|br$/i;
    if (reExcept.test(el.tagName)) {
      return false;
    }

    return !containDynamicContent(el);
  }
}
