import { KoobooComment } from "./koobooComment";

export class Operation {
  constructor(
    public dom: HTMLElement,
    public oldInnerHTML: string,
    public newInnerHTML: string,
    public koobooComment: KoobooComment,
    public koobooId: string | null,
    public actionType: string,
    public commit: string
  ) {}

  undo() {
    this.dom.innerHTML = this.oldInnerHTML;
  }

  redo() {
    this.dom.innerHTML = this.newInnerHTML;
  }
}
