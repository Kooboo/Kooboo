class HoverBorder {
  private readonly _borderWidth = 3;
  private _sourceElement!: HTMLElement;
  private _top!: HTMLElement;
  private _left!: HTMLElement;
  private _right!: HTMLElement;
  private _bottom!: HTMLElement;

  get lines() {
    return [this.top, this.left, this.right, this.bottom];
  }

  get top() {
    if (!this._top) this._top = document.createElement("div");
    this.applyStyle(this._top.style);
    this._top.style.top = this.sourceRect.top - this.bodyRect.top + "px";
    this._top.style.left =
      this.sourceRect.left - this.bodyRect.left + this._borderWidth + "px";
    this._top.style.width =
      this._sourceElement.offsetWidth - this._borderWidth * 2 + "px";
    return this._top;
  }

  get left() {
    if (!this._left) this._left = document.createElement("div");
    this.applyStyle(this._left.style);
    this._left.style.top = this.sourceRect.top - this.bodyRect.top + "px";
    this._left.style.left = this.sourceRect.left - this.bodyRect.left + "px";
    this._left.style.height = this._sourceElement.offsetHeight + "px";
    return this._left;
  }

  get right() {
    if (!this._right) this._right = document.createElement("div");
    this.applyStyle(this._right.style);
    this._right.style.top = this.sourceRect.top - this.bodyRect.top + "px";
    this._right.style.left =
      this.sourceRect.left -
      this.bodyRect.left -
      this._borderWidth +
      this._sourceElement.offsetWidth +
      "px";
    this._right.style.height = this._sourceElement.offsetHeight + "px";
    return this._right;
  }

  get bottom() {
    if (!this._bottom) this._bottom = document.createElement("div");
    this.applyStyle(this._bottom.style);
    this._bottom.style.top =
      this.sourceRect.top -
      this.bodyRect.top +
      this._sourceElement.offsetHeight +
      "px";
    this._bottom.style.left = this.sourceRect.left - this.bodyRect.left + "px";
    this._bottom.style.width = this._sourceElement.offsetWidth + "px";
    return this._bottom;
  }

  get sourceRect() {
    return this._sourceElement.getBoundingClientRect();
  }

  get bodyRect() {
    return this._sourceElement.ownerDocument!.body.getBoundingClientRect();
  }

  updateSource(source: HTMLElement) {
    console.dir(source);
    if (!source || !source.ownerDocument) return;
    console.log(source);
    this._sourceElement = source;
    this.lines.forEach(i => source.ownerDocument!.body.appendChild(i));
  }

  private applyStyle(css: CSSStyleDeclaration) {
    css.position = "absolute";
    css.display = "block";
    css.pointerEvents = "none";
    css.backgroundColor = "#1fb5f6a3";
    css.width = this._borderWidth + "px";
    css.height = this._borderWidth + "px";
    css.zIndex = "999";
  }
}

export default new HoverBorder();
