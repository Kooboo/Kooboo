import { Unit } from "./Unit";
import { getWrapDom } from "@/kooboo/utils";
import { createDiv } from "@/dom/element";

export class DeleteRepeatUnit extends Unit {
  undo(node: Node): void {
    this.newValue = `<!--${node.nodeValue}-->`;
    let temp = createDiv();
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let temp = createDiv();
    let { nodes, startNode } = getWrapDom(node, "repeatitem");
    startNode!.parentNode!.insertBefore(temp, startNode!);
    nodes.forEach(i => temp.appendChild(i));
    this.oldValue = temp.innerHTML;
    temp.outerHTML = this.newValue;
  }
}
