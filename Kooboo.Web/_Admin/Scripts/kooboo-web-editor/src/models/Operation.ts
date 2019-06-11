import { KoobooComment } from "./koobooComment";

export class Operation {
  dom: HTMLElement;
  oldInnerHTML: string;
  newInnerHTML: string;
  koobooComment: KoobooComment;
  koobooId: string | null;
  actionType: string;
  /**
   *
   */
  constructor(
    dom: HTMLElement,
    oldInnerHTML: string,
    newInnerHTML: string,
    koobooComment: KoobooComment,
    koobooId: string | null,
    actionType: string
  ) {
    this.dom = dom;
    this.oldInnerHTML = oldInnerHTML;
    this.newInnerHTML = newInnerHTML;
    this.koobooComment = koobooComment;
    this.koobooId = koobooId;
    this.actionType = actionType;
  }

  undo() {
    this.dom.innerHTML = this.oldInnerHTML;
  }

  redo() {
    this.dom.innerHTML = this.newInnerHTML;
  }
}
