import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  edit,
  copy,
  delete
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {}
