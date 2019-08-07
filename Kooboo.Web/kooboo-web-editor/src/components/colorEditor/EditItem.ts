import { createColorPicker } from "../common/colorPicker";
import { CssColor } from "@/dom/style";

export default class EditItem {
  constructor(protected text: string, protected element: HTMLElement, protected cssColors: CssColor[], protected propName: string) {
    this.inlineCssColor = cssColors.find(item => item.inline) || null;

    this.classCssColor = cssColors.find(item => item.inline == false) || null;
    if (this.classCssColor == null) {
      this.isHideGlobal = true;
    }

    // 如果cssColor为空（没有样式类，也没有内联样式）
    if (cssColors.length == 0) {
      this.color = "#0000";
      return;
    }

    this.color = cssColors[0].value;
    // 如果优先级最高的是样式类
    if (cssColors[0] == this.classCssColor) {
      this.isGlobal = true;
    }

    // 如果是伪类样式
    if (cssColors[0].pseudo != null && cssColors[0].pseudo != "") {
      this.pseudo = cssColors[0].pseudo;
      this.isHideGlobal = true;
    }
  }

  // 伪类
  public pseudo: string = "";

  // 当前颜色
  public color: string;

  // 是否选择全局
  public isGlobal: boolean = false;

  // 是否隐藏全局更新按钮
  protected isHideGlobal: boolean = false;

  protected inlineCssColor: CssColor | null = null;

  protected classCssColor: CssColor | null = null;

  protected CancelGlobalChange() {
    if (this.classCssColor != null) {
      this.classCssColor.cssStyleRule!.style.setProperty(this.classCssColor.prop.prop, this.classCssColor.value);
    }
  }

  protected CancelLocalChange() {
    if (this.pseudo != "") {
      return;
    }

    if (this.inlineCssColor != null) {
      this.element.style.setProperty(this.inlineCssColor.prop.prop, this.inlineCssColor.value);
    } else {
      this.element.style.setProperty(this.propName, "");
    }
  }

  CancelChange(): void {
    this.CancelGlobalChange();
    this.CancelLocalChange();
  }

  protected onGlobalChange(): void {
    if (this.classCssColor == null) {
      return;
    }

    // 清空内联样式
    if (this.inlineCssColor != null) {
      this.element.style.setProperty(this.inlineCssColor.prop.prop, "");
    } else {
      this.element.style.setProperty(this.propName, "");
    }

    // 设置类样式
    this.classCssColor.cssStyleRule!.style.setProperty(
      this.classCssColor.prop.prop,
      this.classCssColor.prop.replaceColor(this.classCssColor.value, this.color)
    );
  }

  protected onLocalChange(): void {
    if (this.pseudo != "") {
      return;
    }

    if (this.inlineCssColor != null) {
      this.element.style.setProperty(this.inlineCssColor.prop.prop, this.inlineCssColor.prop.replaceColor(this.inlineCssColor.value, this.color));
    } else {
      this.element.style.setProperty(this.propName, this.color);
    }
  }

  setChange(): void {
    this.isGlobal == true ? this.onGlobalChange() : this.onLocalChange();
  }

  createPicker() {
    let div = createColorPicker(this.text, this.color, c => {
      this.color = c;
      this.setChange();
    });

    return div;
  }

  createCheckInput() {
    let div = document.createElement("div");
    div.classList.add("kb_web_editor_coloreditor_item_check");

    if (this.isHideGlobal == true) {
      return div;
    }

    let input = document.createElement("input");
    input.type = "checkbox";
    input.checked = this.isGlobal;
    input.onchange = e => {
      this.CancelChange();
      this.isGlobal = (e.currentTarget as HTMLInputElement).checked;
      this.setChange();
    };
    div.appendChild(input);

    let label = document.createElement("label");
    label.innerText = "全局更新";
    div.appendChild(label);

    return div;
  }

  // 返回EditItem的Html元素
  render() {
    let div = document.createElement("div");
    div.classList.add("kb_web_editor_coloreditor_item");
    div.appendChild(this.createPicker());
    div.appendChild(this.createCheckInput());

    return div;
  }
}
