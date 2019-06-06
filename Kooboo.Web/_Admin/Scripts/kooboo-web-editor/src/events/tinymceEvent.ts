import { BaseEvent } from "./BaseEvent";

export class TinymceInputEvent extends BaseEvent<void> {
  constructor() {
    super("TinymceInputEvent");
  }
}
