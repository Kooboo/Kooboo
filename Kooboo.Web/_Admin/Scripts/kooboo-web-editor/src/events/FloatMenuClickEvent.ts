import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  expand,
  edit,
  copy,
  delete,
  editImage
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {}
