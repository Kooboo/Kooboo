export class EditorShade {
  private _sourceElement!: HTMLElement;
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
  }

  get lines() {
    return [this.top, this.left, this.right, this.bottom];
  }

  get top() {
    if (!this._top) {
      this._top = document.createElement("div");
      this.applyStyle(this._top.style);
    }
    this._top.style.left = this.sourceRect.left + "px";
    this._top.style.right = 0 + "px";
    this._top.style.height = this.sourceRect.top - this.bodyRect.top + "px";
    this._top.style.top = 0 + "px";
    return this._top;
  }

  get left() {
    if (!this._left) {
      this._left = document.createElement("div");
      this.applyStyle(this._left.style);
    }
    this._left.style.top = 0 + "px";
    this._left.style.left = 0 + "px";
    this._left.style.width = this.sourceRect.left + "px";
    this._left.style.height =
      this._sourceElement.offsetHeight +
      this.sourceRect.top -
      this.bodyRect.top +
      "px";
    return this._left;
  }

  get right() {
    if (!this._right) {
      this._right = document.createElement("div");
      this.applyStyle(this._right.style);
    }
    this._right.style.top = this.sourceRect.top - this.bodyRect.top + "px";
    this._right.style.left =
      this.sourceRect.left + this.sourceRect.width + "px";
    this._right.style.right = 0 + "px";
    this._right.style.height =
      this._document.body.scrollHeight -
      this.sourceRect.top -
      this.bodyRect.top +
      "px";
    return this._right;
  }

  get bottom() {
    if (!this._bottom) {
      this._bottom = document.createElement("div");
      this.applyStyle(this._bottom.style);
    }
    this._bottom.style.top =
      this.sourceRect.top - this.bodyRect.top + this.sourceRect.height + "px";
    this._bottom.style.left = 0 + "px";
    this._bottom.style.width =
      this._sourceElement.offsetWidth + this.sourceRect.left + "px";
    this._bottom.style.height =
      this._document.body.scrollHeight -
      this.sourceRect.top -
      this.bodyRect.top +
      "px";
    return this._bottom;
  }

  get sourceRect() {
    return this._sourceElement.getBoundingClientRect();
  }

  get bodyRect() {
    return this._document.body.getBoundingClientRect();
  }

  updateSource(source: HTMLElement) {
    if (!source) return;
    this._sourceElement = source;
    this.lines.forEach(i => this._document.body.appendChild(i));
    console.log(this.bodyRect);
  }

  private applyStyle(css: CSSStyleDeclaration) {
    css.position = "absolute";
    css.display = "block";
    css.pointerEvents = "none";
    css.backgroundColor = "rgba(0,0,0,0.3)";
    css.zIndex = "999";
  }

  clear(document: Document) {
    this.lines.forEach(i => {
      if (i.parentElement) {
        this._document.body.removeChild(i);
      }
    });
  }
}
