import { KoobooComment } from "./koobooComment";

export class Operation {
  dom: HTMLElement;
  oldInnerHTML: string;
  newInnerHTML: string;
  koobooComment: KoobooComment;
  koobooId: string | null;

  /**
   *
   */
  constructor(
    dom: HTMLElement,
    oldInnerHTML: string,
    newInnerHTML: string,
    koobooComment: KoobooComment,
    koobooId: string | null
  ) {
    this.dom = dom;
    this.oldInnerHTML = oldInnerHTML;
    this.newInnerHTML = newInnerHTML;
    this.koobooComment = koobooComment;
    this.koobooId = koobooId;
  }

  undo() {
    this.dom.scrollIntoView();
    this.dom.innerHTML = this.oldInnerHTML;
  }

  redo() {
    this.dom.scrollIntoView();
    this.dom.innerHTML = this.newInnerHTML;
  }
}
