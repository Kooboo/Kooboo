import { BaseItem } from "./base";
import { TEXT } from "../../../lang";
import { MenuActions } from "../../../events/FloatMenuClickEvent";
import { containDynamicContent, getAllElement } from "../../../common/dom";
import context from "../../../context";
import { Operation } from "../../../models/Operation";

export class CopyItem extends BaseItem {
  text: string = TEXT.COPY;
  type: MenuActions = MenuActions.copy;
  canShow(): boolean {
    let args = context.lastSelectedDomEventArgs;
    if (!args) return false;
    let el = args.element;

    if (el.tagName.toLowerCase() == "body") return false;
    if (!args.parentKoobooId) return false;

    return !containDynamicContent(el);
  }

  click(e: MouseEvent) {
    let args = context.lastSelectedDomEventArgs;
    if (!args || !args.closeParent) return false;
    // getAllElement(args.closeParent);
    // let operationLog = new Operation(
    //   args.closeParent,
    //   args.closeParent.innerHTML
    // );
  }
}
