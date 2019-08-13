export class ColumnRule {
  constructor(public value: string) {
    let temp = document.createElement("div");
    temp.style.columnRule = value;

    this.width = temp.style.columnRuleWidth!;
    this.style = temp.style.columnRuleStyle!;
    this.color = temp.style.columnRuleColor!;
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
