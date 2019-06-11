import { createContainer } from "./container";
import { createTitle } from "./menuItems/title";
import { EditItem } from "./menuItems/edit";
import context from "../../context";
import { CopyItem } from "./menuItems/copy";

export class FloatMenu {
  private _container: HTMLDivElement;
  private _document: Document;

  constructor(document: Document) {
    this._container = createContainer(document);
    this._document = document;
  }

  update(x: number, y: number) {
    this._container.style.top = y + "px";
    this._container.style.left = x + "px";
    this._container.innerHTML = "";
    this._container.appendChild(createTitle(document));

    this._container.appendChild(
      new EditItem(document, context.lastSelectedDomEventArgs).el
    );
    this._container.appendChild(
      new CopyItem(document, context.lastSelectedDomEventArgs).el
    );

    document.body.appendChild(this._container);
  }

  clear() {
    if (this._container && this._container.parentElement) {
      this._document.body.removeChild(this._container);
    }
  }
}
