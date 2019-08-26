import { Unit } from "./Unit";

export class StyleUnit extends Unit {
  constructor(public oldValue: string, public prop: string, public important: string) {
    super(oldValue);
  }

  undo(node: Node): void {
    let el = node as HTMLElement;
    this.newValue = el.style.getPropertyValue(this.prop);
    el.style.setProperty(this.prop, this.oldValue, this.important);
  }
  redo(node: Node): void {
    let el = node as HTMLElement;
    this.oldValue = el.style.getPropertyValue(this.prop);
    el.style.setProperty(this.prop, this.newValue, this.important);
  }
}
