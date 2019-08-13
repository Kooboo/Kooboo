export class Outline {
  constructor(public value: string) {
    let temp = document.createElement("div");
    temp.style.outline = value;

    this.width = temp.style.outlineWidth!;
    this.style = temp.style.outlineStyle!;
    this.color = temp.style.outlineColor!;
  }

  color: string;
  style: string;
  width: string;

  toString() {
    let result = "";
    if (this.width && this.width != "initial") result += `${this.width} `;
    if (this.style && this.style != "initial") result += `${this.style} `;
    if (this.color && this.color != "initial") result += `${this.color} `;

    return result;
  }
}
