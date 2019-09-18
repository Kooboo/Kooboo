import { Unit } from "./Unit";

export class InnerHtmlUnit extends Unit {
  undo(node: Node): void {
    let el = node as HTMLElement;
    this.newValue = el.innerHTML;
    el.innerHTML = this.oldValue;
  }
  redo(node: Node): void {
    let el = node as HTMLElement;
    this.oldValue = el.innerHTML;
    el.innerHTML = this.newValue;
  }
}
