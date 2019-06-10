import { BaseEvent } from "./BaseEvent";

export enum MenuActions {
  close,
  edit
}

export class FloatMenuClickEvent extends BaseEvent<MenuActions> {
  /**
   *
   */
  constructor() {
    super("FloatMenuClickEvent");
  }
}
