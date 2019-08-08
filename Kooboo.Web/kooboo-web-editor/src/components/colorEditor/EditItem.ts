import { createColorPicker } from "../common/colorPicker";
import { CssColor } from "@/dom/style";

export default class EditItem {
  constructor(
    protected text: string,
    protected element: HTMLElement,
    protected cssColors: CssColor[],
    protected propName: string) {

    this.inlineCssColor = cssColors.find(item => item.inline) || null;

    this.classCssColor = cssColors.find(item => item.inline == false) || null;

    if (this.classCssColor == null) {
      this.isHideGlobal = true;
    }

    // 没有样式类，也没有内联样式
    if (this.inlineCssColor == null && this.classCssColor == null) {
      return;
    }

    let firstCssColor = cssColors[0];

    this.color = firstCssColor.value;
    // 如果优先级最高的是样式类
    if (firstCssColor == this.classCssColor) {
      this.isGlobal = true;
    }

    // 如果是伪类样式
    if (firstCssColor.pseudo != null && firstCssColor.pseudo != "") {
      this.pseudo = firstCssColor.pseudo;
      this.isHideGlobal = true;
    }
  }

  public localEditData: LocalEditData | null = null;

  public globalEditData: GlobalEditData | null = null;

  // 伪类
  public pseudo: string = "";

  // 当前颜色
  public color: string = "#0000";

  // 是否选择全局
  public isGlobal: boolean = false;

  // 是否隐藏全局更新按钮
  protected isHideGlobal: boolean = false;

  // 是否改变了颜色
  protected isChangeColor: boolean = false;

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
      this.element.style.removeProperty(this.propName);
    }
  }

  CancelChange(): void {
    this.globalEditData = null;
    this.localEditData = null;
    this.CancelGlobalChange();
    this.CancelLocalChange();
  }

  protected onGlobalChange(): void {
    if (this.classCssColor == null) {
      return;
    }

    // 清空内联样式
    if(this.pseudo == ""){
      // 如果inlineCssColor不为空，则元素在编辑前存在内联样式
      if (this.inlineCssColor != null) {
        this.element.style.removeProperty(this.inlineCssColor.prop.prop);
        this.localEditData = {
          value: "",
          propName: this.inlineCssColor.prop.prop
        }
      } else {
        this.element.style.removeProperty(this.propName);
      }
    }

    // 设置类样式
    this.classCssColor.cssStyleRule!.style.setProperty(
      this.classCssColor.prop.prop,
      this.classCssColor.prop.replaceColor(this.classCssColor.value, this.color)
    );

    this.globalEditData = {
      value: this.classCssColor.prop.replaceColor(this.classCssColor.value, this.color),
      propName: this.classCssColor.prop.prop,
      pseudo: this.pseudo,
      selector: this.classCssColor.targetSelector,
      styleTagKoobooId: this.classCssColor.koobooId,
      styleSheetUrl: this.classCssColor.url == null ? "" : this.classCssColor.url
    }
  }

  protected onLocalChange(): void {
    if (this.pseudo != "") {
      return;
    }

    let value;
    let propName;
    if (this.inlineCssColor != null) {
      value = this.inlineCssColor.prop.replaceColor(this.inlineCssColor.value, this.color);
      propName = this.inlineCssColor.prop.prop;
      this.element.style.setProperty(propName, value);
    } else {
      value = this.color;
      propName = this.propName;
      this.element.style.setProperty(propName, value);
    }

    this.localEditData = {
      value: value,
      propName: propName
    }
  }

  setChange(): void {
    // 如果颜色没有做更改
    if(this.isChangeColor == false){
      return;
    }
    this.isGlobal == true ? this.onGlobalChange() : this.onLocalChange();
  }

  resetElement(element: HTMLElement)
  {
    this.element = element;
  }

  createPicker() {
    let div = createColorPicker(this.text, this.color, c => {
      this.color = c;
      this.isChangeColor = true;
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
      this.isChangeColor = true;
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

export class GlobalEditData{
  value: string = "";
  propName: string = "";
  pseudo: string = "";
  selector: string = "";
  styleTagKoobooId: string = "";
  styleSheetUrl: string = "";
}

export class LocalEditData{
  value: string = "";
  propName: string = "";
}