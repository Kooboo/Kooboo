import { BaseItem } from "./BaseItem";
import { TEXT } from "../../lang";
import { MenuActions } from "../../events/FloatMenuClickEvent";

export class DeleteItem extends BaseItem {
  text: string = TEXT.DELETE;
  type: MenuActions = MenuActions.delete;
  canShow(): boolean {
    throw new Error("Method not implemented.");
  }
}
