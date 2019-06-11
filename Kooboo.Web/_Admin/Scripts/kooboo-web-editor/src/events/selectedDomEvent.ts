import { KoobooComment } from "../models/KoobooComment";
import { BaseEvent } from "./BaseEvent";

export class SelectedDomEventArgs {
  constructor(
    public element: HTMLElement,
    public closeElement: HTMLElement,
    public koobooId: string | null,
    public koobooComments: KoobooComment[]
  ) {}
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {
  constructor() {
    super("SelectedDomEvent");
  }
}
