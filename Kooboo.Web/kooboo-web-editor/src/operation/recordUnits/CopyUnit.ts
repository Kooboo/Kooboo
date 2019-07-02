import { Unit } from "./Unit";

export class CopyUnit extends Unit {
  undo(node: Node): void {
    let el = node as HTMLElement;
    this.newValue = el.outerHTML;
    el.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let temp = document.createElement("div");
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.newValue;
  }
}
