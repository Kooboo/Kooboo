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

  get editableComment() {
    return this.koobooComments.find(f => {
      if (!f.objecttype) return false;
      var objecttype = f.objecttype.toLowerCase();

      return (
        objecttype != OBJECT_TYPE.contentrepeater &&
        objecttype != OBJECT_TYPE.attribute
      );
    });
  }

  get deletableComment() {
    if (
      this.koobooComments.some(s => s.objecttype == OBJECT_TYPE.contentrepeater)
    ) {
      return undefined;
    }

    return this.koobooComments.find(f => {
      if (!f.objecttype) return false;
      var objecttype = f.objecttype.toLowerCase();

      return (
        objecttype != OBJECT_TYPE.contentrepeater &&
        objecttype != OBJECT_TYPE.attribute &&
        objecttype != OBJECT_TYPE.label
      );
    });
  }
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {}
