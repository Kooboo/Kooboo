export class Border {
    constructor(public value: string) {
        let temp = document.createElement("div");
        temp.style.border = value;

        this.width = temp.style.borderWidth;
        this.style = temp.style.borderStyle;
        this.color = temp.style.borderColor;
    }

    width: string | null;
    style: string | null;
    color: string | null;

    toString() {
        let result = "";
        if (this.width && this.width != "initial")
            result += `${this.width} `;

        if (this.style && this.style != "initial")
            result += `${this.style} `;

        if (this.color && this.color != "initial")
            result += `${this.color} `;

        return result;
    }
}