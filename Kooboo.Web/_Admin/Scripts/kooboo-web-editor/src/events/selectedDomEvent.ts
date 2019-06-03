import { KoobooComment } from "../models/koobooComment";
import { BaseEvent } from "./baseEvent";

export class SelectedDomEventArgs {
  element: HTMLElement;
  closeElement: HTMLElement;
  koobooId: string | null;
  koobooComment: KoobooComment;
  lastSelectedDomEventArgs: SelectedDomEventArgs | undefined;

  constructor(
    element: HTMLElement,
    closeElement: HTMLElement,
    koobooId: string | null,
    koobooComment: KoobooComment
  ) {
    this.element = element;
    this.closeElement = closeElement;
    this.koobooId = koobooId;
    this.koobooComment = koobooComment;
  }
}

export class SelectedDomEvent extends BaseEvent<SelectedDomEventArgs> {
  constructor() {
    super("SelectedDomEvent");
  }
}
