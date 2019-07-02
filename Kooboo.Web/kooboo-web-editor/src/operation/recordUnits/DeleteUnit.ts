import { Unit } from "./Unit";

export class DeleteUnit extends Unit {
  undo(node: Node): void {
    this.newValue = `<!--${node.nodeValue}-->`;
    let temp = document.createElement("div");
    node.parentNode!.replaceChild(temp, node);
    temp.outerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let el = node as HTMLElement;
    el.outerHTML = this.newValue;
  }
}
