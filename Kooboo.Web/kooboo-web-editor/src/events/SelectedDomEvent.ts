import { BaseEvent } from "./BaseEvent";
import context from "../common/context";
import { KOOBOO_ID } from "@/common/constants";

export class SelectedDomEventArgs {
  constructor(public element: HTMLElement) {}
  get koobooId() {
    return this.element.getAttribute(KOOBOO_ID);
  }
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {
  emit(args: SelectedDomEventArgs) {
    context.lastSelectedDomEventArgs = args;
    super.emit(args);
  }
}
