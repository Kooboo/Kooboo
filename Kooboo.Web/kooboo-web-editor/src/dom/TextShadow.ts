export class TextShadow {
  constructor(public value: string) {
    // let temp = document.createElement("div");
    // temp.style.boxShadow = value;

    value = value.replace(";", "");

    value = value.replace(/\s+/, " ");
    let values = value.trim().split(" ");
    if (values.length < 3 || values.length > 4) {
      return;
    }

    if (values.length == 3) {
      this.hShadow = values[0];
      this.vShadow = values[1];
      this.color = values[2];
    }

    if (values.length == 4) {
      this.hShadow = values[0];
      this.vShadow = values[1];
      this.blur = values[2];
      this.color = values[3];
    }
  }

  hShadow: string = "0px";
  vShadow: string = "0px";
  blur: string = "0px";
  color: string = "#fff0";

  toString() {
    let result = `${this.hShadow} ${this.vShadow} ${this.blur} ${this.color}`;

    return result;
  }
}
