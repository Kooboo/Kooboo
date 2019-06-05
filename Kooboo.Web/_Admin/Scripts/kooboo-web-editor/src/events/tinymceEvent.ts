import { BaseEvent } from "./BaseEvent";
export class TinymceDisplayEvent extends BaseEvent<boolean> {
  constructor() {
    super("TinymceDisplayEvent");
  }
}

export class TinymceInputEvent extends BaseEvent<void> {
  constructor() {
    super("TinymceInputEvent");
  }
}
