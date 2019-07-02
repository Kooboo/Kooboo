import { Unit } from "./Unit";
import { getWrapDom } from "@/kooboo/utils";
import { OBJECT_TYPE } from "@/common/constants";

export class DeleteRepeatUnit extends Unit {
  undo(node: Node): void {
    this.newValue = `<!--${node.nodeValue}-->`;
    let temp = document.createElement("div");
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let temp = document.createElement("div");
    let { nodes, startNode } = getWrapDom(node, OBJECT_TYPE.contentrepeater);
    startNode!.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    this.oldValue = temp.innerHTML;
    temp.outerHTML = this.newValue;
  }
}
