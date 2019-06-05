import { getMaxHeight } from "../dom/domAnalyze";
import { EDITOR_SHADE_COLOR, STANDARD_Z_INDEX } from "../constants";

export class EditorShade {
  private _top!: HTMLElement;
  private _left!: HTMLElement;
  private _right!: HTMLElement;
  private _bottom!: HTMLElement;
  private _document: Document;

  /**
   *
   */
  constructor(document: Document) {
    this._document = document;
    this._top = document.createElement("div");
    this._left = document.createElement("div");
    this._right = document.createElement("div");
    this._bottom = document.createElement("div");
    this.blocks.forEach(i => this.initBlock(i));
  }

  get blocks() {
    return [this._top, this._left, this._right, this._bottom];
  }

  updateSource(source: HTMLElement) {
    if (!source || !this._top) return;

    let sourceRect = source.getBoundingClientRect();
    let bodyRect = this._document.body.getBoundingClientRect();

    this._top.style.left = 0 + "px";
    this._top.style.right = 0 + "px";
    this._top.style.height = sourceRect.top - bodyRect.top + "px";
    this._top.style.top = 0 + "px";

    this._left.style.top = sourceRect.top - bodyRect.top + "px";
    this._left.style.left = 0 + "px";
    this._left.style.width = sourceRect.left + "px";
    this._left.style.height = sourceRect.height + "px";

    this._right.style.top = sourceRect.top - bodyRect.top + "px";
    this._right.style.left = sourceRect.left + sourceRect.width + "px";
    this._right.style.right = 0 + "px";
    this._right.style.height = sourceRect.height + "px";

    this._bottom.style.top =
      sourceRect.top - bodyRect.top + sourceRect.height + "px";
    this._bottom.style.left = 0 + "px";
    this._bottom.style.right = 0 + "px";
    this._bottom.style.height =
      getMaxHeight(this._document) -
      (sourceRect.top - bodyRect.top + sourceRect.height) +
      "px";

    this.blocks.forEach(i => this._document.body.appendChild(i));
  }

  private initBlock(element: HTMLElement) {
    let css = element.style;
    css.position = "absolute";
    css.display = "block";
    css.backgroundColor = EDITOR_SHADE_COLOR;
    css.zIndex = STANDARD_Z_INDEX + "";
    css.cursor = "not-allowed";

    element.onclick = e => {
      e.stopPropagation();
    };
  }

  clear() {
    this.blocks.forEach(i => {
      if (i && i.parentElement) {
        this._document.body.removeChild(i);
      }
    });
  }
}
