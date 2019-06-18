import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  expand,
  edit,
  copy,
  delete,
  editImage,
  editLink
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {}
