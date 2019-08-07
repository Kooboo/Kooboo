export class Outline {
  constructor(public value: string) {
    let temp = document.createElement("div");
    temp.style.border = value;

    this.width = temp.style.borderWidth!;
    this.style = temp.style.borderStyle!;
    this.color = temp.style.borderColor!;
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
