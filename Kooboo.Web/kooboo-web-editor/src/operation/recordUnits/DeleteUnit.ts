import { Unit } from "./Unit";
import { createDiv } from "@/dom/element";

export class DeleteUnit extends Unit {
  undo(node: Node): void {
    this.newValue = `<!--${node.nodeValue}-->`;
    let temp = createDiv();
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let el = node as HTMLElement;
    el.outerHTML = this.newValue;
  }
}
