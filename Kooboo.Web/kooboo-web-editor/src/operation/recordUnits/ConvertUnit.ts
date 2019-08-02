import { Unit } from "./Unit";
import { getWrapDom } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";

export class ConvertUnit extends Unit {
    constructor(oldValue: string, public objectType: string)
    {
        super(oldValue);
    }

    undo(node: Node): void {
        let el = node as HTMLElement;
        this.newValue = el.outerHTML;

        let oldNode = this.createNodesForString(this.oldValue)[0];
        el.parentElement!.replaceChild(oldNode, el);
    }

    redo(node: Node): void {
        let el = node as HTMLElement;

        let oldNode = this.createNodesForString(this.newValue)[0];
        el.parentElement!.replaceChild(oldNode, el);
    }

    createNodesForString(htmlStr:string){
        let div = document.createElement("div");

        div.innerHTML = htmlStr;
        return div.childNodes;
    }

    static CreateViewConvertUnit(oldValue: string) {
        return new ConvertUnit(oldValue, OBJECT_TYPE.view);
    }
}
