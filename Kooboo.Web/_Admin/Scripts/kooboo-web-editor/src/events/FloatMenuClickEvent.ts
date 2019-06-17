import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  edit,
  copy,
  delete,
  editImage
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {}
