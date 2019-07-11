import { Unit } from "./Unit";
import { getWrapDom } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";
import createDiv from "@/dom/div";

export class CopyRepeatUnit extends Unit {
  undo(node: Node): void {
    let temp = createDiv();
    let { nodes, startNode } = getWrapDom(node, OBJECT_TYPE.contentrepeater);
    startNode!.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    this.newValue = temp.innerHTML;
    temp.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let temp = createDiv();
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.newValue;
  }
}
