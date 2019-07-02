import { Unit } from "./Unit";
import { getWrapDom } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";

export class CopyRepeatUnit extends Unit {
  undo(node: Node): void {
    let temp = document.createElement("div");
    let { nodes, startNode } = getWrapDom(node, OBJECT_TYPE.contentrepeater);
    node.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    this.newValue = temp.innerHTML;
    temp.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let temp = document.createElement("div");
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.newValue;
  }
}
