import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  edit,
  copy,
  delete,
  replaceToImage
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {}
