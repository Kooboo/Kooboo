import { BaseEvent } from "./BaseEvent";
import context from "../common/context";

export class SelectedDomEventArgs {
  constructor(public element: HTMLElement, public koobooId: string | null) {}
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {
  emit(args: SelectedDomEventArgs) {
    context.lastSelectedDomEventArgs = args;
    super.emit(args);
  }
}
