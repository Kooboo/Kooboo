import { TEXT } from "../../../lang";
import { KoobooComment } from "../../../models/KoobooComment";
import { BaseItem } from "./base";
import { MenuActions } from "../../../events/FloatMenuClickEvent";

export class EditItem extends BaseItem {
  type: MenuActions;
  text: string;
  canShow(): boolean {
    return true;
  }
  constructor(
    readonly document: Document,
    readonly comment: KoobooComment,
    readonly koobooId: string | null
  ) {
    super(document);
    this.text = TEXT.EDIT;
    this.type = MenuActions.edit;
  }
}
