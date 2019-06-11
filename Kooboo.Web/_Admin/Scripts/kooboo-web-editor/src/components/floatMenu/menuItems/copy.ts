import { BaseItem } from "./base";
import { TEXT } from "../../../lang";
import { MenuActions } from "../../../events/FloatMenuClickEvent";
import { SelectedDomEventArgs } from "../../../events/SelectedDomEvent";

export class CopyItem extends BaseItem {
  text: string = TEXT.COPY;
  type: MenuActions = MenuActions.copy;
  canShow(selectedDomEventArgs: SelectedDomEventArgs | undefined): boolean {
    if (!selectedDomEventArgs) return false;
    let el = selectedDomEventArgs.closeElement;

    if (el.tagName.toLowerCase() == "body") return false;

    return true;
  }
}
