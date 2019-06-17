import { BaseEvent } from "./BaseEvent";
import context from "../context";

export class HoverDomEventArgs {
  constructor(
    public element: HTMLElement,
    public closeElement: HTMLElement | undefined
  ) {}
}

export class HoverDomEvent extends BaseEvent<HoverDomEventArgs> {
  emit(arg: HoverDomEventArgs) {
    context.lastHoverDomEventArgs = arg;
    super.emit(arg);
  }
}
