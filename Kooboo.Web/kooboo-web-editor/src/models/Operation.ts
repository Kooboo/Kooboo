import { KoobooComment } from "./koobooComment";
import { getAllElement } from "../dom/utils";
import { KOOBOO_GUID, EDITOR_TYPE } from "../common/constants";

export class Operation {
  constructor(
    public guid: string,
    public oldInnerHTML: string,
    public newInnerHTML: string,
    public koobooComment: KoobooComment,
    public koobooId: string | null,
    public actionType: string,
    public commit: string,
    public editorType: string
  ) {}

  undo(document: Document) {
    for (const element of getAllElement(document.body)) {
      if (element.hasAttribute(KOOBOO_GUID)) {
        if (element.getAttribute(KOOBOO_GUID) == this.guid) {
          if (this.editorType == EDITOR_TYPE.style) {
            element.setAttribute("style", this.oldInnerHTML);
            return;
          }

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
          if (this.editorType == EDITOR_TYPE.style) {
            element.setAttribute("style", this.newInnerHTML);
            return;
          }

          this.oldInnerHTML = element.innerHTML;
          element.innerHTML = this.newInnerHTML;
          return;
        }
      }
    }
  }
}
