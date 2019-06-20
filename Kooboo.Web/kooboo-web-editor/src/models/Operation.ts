import { KoobooComment } from "./koobooComment";
import { getAllElement } from "../dom/utils";
import { KOOBOO_GUID } from "../common/constants";

export class Operation {
  constructor(
    public guid: string,
    public oldInnerHTML: string,
    public newInnerHTML: string,
    public koobooComment: KoobooComment,
    public koobooId: string | null,
    public actionType: string,
    public commit: string
  ) {}

  undo(document: Document) {
    for (const element of getAllElement(document.body)) {
      if (element.hasAttribute(KOOBOO_GUID)) {
        if (element.getAttribute(KOOBOO_GUID) == this.guid) {
          this.newInnerHTML = element.innerHTML;
          element.innerHTML = this.oldInnerHTML;
          return;
        }
      }
    }
  }

  redo(document: Document) {
    for (const element of getAllElement(document.body)) {
      if (element.hasAttribute(KOOBOO_GUID)) {
        if (element.getAttribute(KOOBOO_GUID) == this.guid) {
          this.oldInnerHTML = element.innerHTML;
          element.innerHTML = this.newInnerHTML;
          return;
        }
      }
    }
  }
}
