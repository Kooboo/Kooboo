import { createContainer } from "./container";
import { createTitle } from "./title";
import { EditItem } from "./EditItem";
import { CopyItem } from "./CopyItem";
import { BaseItem } from "./BaseItem";

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
    this.addItem(new EditItem(document));
    this.addItem(new CopyItem(document));

    if (this._container.children.length > 1) {
      document.body.appendChild(this._container);
    }
  }
  private addItem(item: BaseItem) {
    var el = item.el;
    if (el) {
      this._container.appendChild(el);
    }
  }

  clear() {
    if (this._container && this._container.parentElement) {
      this._document.body.removeChild(this._container);
    }
  }
}
