import { BaseEvent } from "./baseEvent";
export class TinymceEvent extends BaseEvent<boolean> {
  constructor() {
    super("TinymceEvent");
  }
}
