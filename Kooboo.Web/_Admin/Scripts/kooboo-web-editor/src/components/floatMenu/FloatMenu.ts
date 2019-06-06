import { createContainer } from "./container";
import { createTitle } from "./title";
import { createBaseItem } from "./menuItems/base";

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
    this._container.appendChild(createBaseItem(document));
    this._container.appendChild(createBaseItem(document));
    document.body.appendChild(this._container);
  }

  clear() {
    if (this._container && this._container.parentElement) {
      this._document.body.removeChild(this._container);
    }
  }
}
