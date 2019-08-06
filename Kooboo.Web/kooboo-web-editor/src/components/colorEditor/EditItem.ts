import { createColorPicker } from "../common/colorPicker";

export default class EditItem{
    constructor(protected text:string, protected oldColor:string, protected onSave: (color: string) => void = (c)=>{})
    {
        this.color = oldColor;
        this.isGlobal = false;
    }

    // 当前的颜色
    public color: string;

    // 是否选择全局
    public isGlobal: boolean;

    createPicker(){
        let div = createColorPicker(this.text, this.oldColor, c => { this.color = c; this.onSave(c); });

        return div;
    }

    createCheckInput(){
        let div = document.createElement("div");
        div.classList.add("kb_web_editor_coloreditor_item_check");

        let input = document.createElement("input");
        input.type = "checkbox";
        input.onchange = (e)=>{
            this.isGlobal = (e.currentTarget as HTMLInputElement).checked;
        };
        div.appendChild(input);

        let label = document.createElement("label");
        label.innerText = "全局更新";
        div.appendChild(label);

        return div;
    }

    // 返回EditItem的Html元素
    render(){
        let div = document.createElement("div");
        div.classList.add("kb_web_editor_coloreditor_item");
        div.appendChild(this.createPicker());
        div.appendChild(this.createCheckInput());

        return div;
    }
}