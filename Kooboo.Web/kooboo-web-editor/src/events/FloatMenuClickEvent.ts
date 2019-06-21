import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  expand,
  edit,
  copy,
  delete,
  editImage,
  editLink,
  jumpLink,
  editHtmlBlock
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {}
