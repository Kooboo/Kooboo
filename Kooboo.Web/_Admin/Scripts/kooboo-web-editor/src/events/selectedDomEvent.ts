import { KoobooComment } from "../models/KoobooComment";
import { BaseEvent } from "./BaseEvent";

export class SelectedDomEventArgs {
  constructor(
    public element: HTMLElement,
    public koobooId: string,
    public closeParent: HTMLElement | null,
    public parentKoobooId: string | null,
    public koobooComments: KoobooComment[],
    public mouseEvent: MouseEvent
  ) {}
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {
  constructor() {
    super("SelectedDomEvent");
  }
}
