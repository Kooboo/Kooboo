import { BaseEvent } from "./BaseEvent";

export class HoverDomEventArgs {
  constructor(
    public element: HTMLElement,
    public closeElement: HTMLElement | undefined
  ) {}
}

export class HoverDomEvent extends BaseEvent<HoverDomEventArgs> {}
