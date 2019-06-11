import { TEXT } from "../../../lang";
import { BaseItem } from "./base";
import { MenuActions } from "../../../events/FloatMenuClickEvent";
import { SelectedDomEventArgs } from "../../../events/SelectedDomEvent";
import { OBJECT_TYPE } from "../../../constants";
import { containDynamicContent } from "../../../helpers/domAnalyze";

export class EditItem extends BaseItem {
  type: MenuActions;
  text: string;

  canShow(selectedDomEventArgs: SelectedDomEventArgs | undefined): boolean {
    if (
      !selectedDomEventArgs ||
      selectedDomEventArgs.koobooComments.length == 0 ||
      !selectedDomEventArgs.closeElement
    ) {
      return false;
    }
    let comment = selectedDomEventArgs.koobooComments[0];
    let el = selectedDomEventArgs.closeElement;
    let id = selectedDomEventArgs.koobooId;

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

  constructor(
    readonly document: Document,
    readonly selectedDomEventArgs: SelectedDomEventArgs | undefined
  ) {
    super(document, selectedDomEventArgs);
    this.text = TEXT.EDIT;
    this.type = MenuActions.edit;
  }
}
