export class BoxShadow {
  constructor(public value: string) {
    // let temp = document.createElement("div");
    // temp.style.boxShadow = value;

    value = value.replace(";", "");
    if (value.indexOf("inset") > -1) {
      value = value.replace("inset", "");
      this.inset = "inset";
    }

    value = value.replace(/\s+/g, " ");
    value = value.replace(/,\s+/g, ","); // rgb(0, 0, 0) 移除其中的空格
    let values = value.trim().split(" ");
    if (values.length < 3 || values.length > 5) {
      return;
    }

    // 判断是否以颜色开头
    let colorpos, hShadowpos, vShadowpos, blurpos, spreadpos;
    if (/^([a-zA-Z#]|rgb)/.test(values[0])) {
      colorpos = 0;
      hShadowpos = 1;
      vShadowpos = 2;
      blurpos = 3;
      spreadpos = 4;
    } else {
      hShadowpos = 0;
      vShadowpos = 1;
      blurpos = 2;
      spreadpos = 3;
      colorpos = values.length - 1;
    }

    if (values.length == 3) {
      this.hShadow = values[hShadowpos];
      this.vShadow = values[vShadowpos];
      this.color = values[colorpos];
    }

    if (values.length == 4) {
      this.hShadow = values[hShadowpos];
      this.vShadow = values[vShadowpos];
      this.blur = values[blurpos];
      this.color = values[colorpos];
    }

    if (values.length == 5) {
      this.hShadow = values[hShadowpos];
      this.vShadow = values[vShadowpos];
      this.blur = values[blurpos];
      this.spread = values[spreadpos];
      this.color = values[colorpos];
    }
  }

  hShadow: string = "0px";
  vShadow: string = "0px";
  blur: string = "0px";
  spread: string = "0px";
  color: string = "#fff0";
  inset: string | null = null;

  toString() {
    let result = `${this.hShadow} ${this.vShadow} ${this.blur} ${this.spread} ${this.color} ${this.inset || ""}`;

    return result;
  }
}
