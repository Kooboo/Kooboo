import { KoobooComment } from "../models/KoobooComment";
import { BaseEvent } from "./BaseEvent";
import { OBJECT_TYPE } from "../common/constants";
import context from "../common/context";

export class SelectedDomEventArgs {
  constructor(
    public element: HTMLElement,
    public koobooId: string | null,
    public closeParent: HTMLElement | null,
    public parentKoobooId: string | null,
    public koobooComments: KoobooComment[]
  ) {}
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {
  emit(args: SelectedDomEventArgs) {
    context.lastSelectedDomEventArgs = args;
    super.emit(args);
  }
}