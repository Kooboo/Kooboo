import { KoobooComment } from "../models/KoobooComment";
import { BaseEvent } from "./BaseEvent";
import { OBJECT_TYPE } from "../constants";

export class SelectedDomEventArgs {
  constructor(
    public element: HTMLElement,
    public koobooId: string | null,
    public closeParent: HTMLElement | null,
    public parentKoobooId: string | null,
    public koobooComments: KoobooComment[],
    public mouseEvent: MouseEvent
  ) {}

  get editComment() {
    return this.koobooComments.find(
      f =>
        f.objecttype != OBJECT_TYPE.contentrepeater &&
        f.objecttype != OBJECT_TYPE.attribute
    );
  }
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {}
