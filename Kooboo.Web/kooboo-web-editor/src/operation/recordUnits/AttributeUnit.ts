import { Unit } from "./Unit";

export class AttributeUnit extends Unit {
  constructor(public oldValue: string, public attribute: string) {
    super(oldValue);
  }

  undo(node: Node): void {
    let el = node as HTMLElement;
    this.newValue = el.getAttribute(this.attribute)!;
    el.setAttribute(this.attribute, this.oldValue);
  }
  redo(node: Node): void {
    let el = node as HTMLElement;
    this.oldValue = el.getAttribute(this.attribute)!;
    el.setAttribute(this.attribute, this.newValue);
  }
}
