import { Unit } from "./Unit";
import { createDiv } from "@/dom/element";

export class CopyUnit extends Unit {
  undo(node: Node): void {
    let el = node as HTMLElement;
    this.newValue = el.outerHTML;
    el.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let temp = createDiv();
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.newValue;
  }
}
