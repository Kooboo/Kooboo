import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  edit,
  copy
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {
  /**
   *
   */
  constructor() {
    super("FloatMenuClickEvent");
  }
}
