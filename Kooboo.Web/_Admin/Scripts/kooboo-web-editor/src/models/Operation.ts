import { KoobooComment } from "./koobooComment";
import { getAllElement } from "../common/dom";

export class Operation {
  constructor(
    public koobooId: string,
    public oldInnerHTML: string,
    public newInnerHTML: string,
    public koobooComment: KoobooComment,
    public commitKoobooId: string | null,
    public actionType: string,
    public commit: string
  ) {}

  undo(document: Document) {
    for (const element of getAllElement(document.body)) {
      if (element.hasAttribute("kooboo-id")) {
        if (element.getAttribute("kooboo-id") == this.koobooId) {
          element.innerHTML = this.oldInnerHTML;
          return;
        }
      }
    }
  }

  redo(document: Document) {
    for (const element of getAllElement(document.body)) {
      if (element.hasAttribute("kooboo-id")) {
        if (element.getAttribute("kooboo-id") == this.koobooId) {
          element.innerHTML = this.newInnerHTML;
          return;
        }
      }
    }
  }
}
