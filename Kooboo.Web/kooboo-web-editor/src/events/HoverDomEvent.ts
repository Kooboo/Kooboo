import { BaseEvent } from "./BaseEvent";
import context from "../common/context";

export class HoverDomEventArgs {
  constructor(public element: HTMLElement, public closeElement: HTMLElement) {}
}

export class HoverDomEvent extends BaseEvent<HoverDomEventArgs> {
  emit(arg: HoverDomEventArgs) {
    context.lastHoverDomEventArgs = arg;
    super.emit(arg);
  }
}
