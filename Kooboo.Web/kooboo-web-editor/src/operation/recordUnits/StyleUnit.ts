import { Unit } from "./Unit";

export class StyleUnit extends Unit {
  constructor(public oldValue: string, public prop: string) {
    super(oldValue);
  }

  undo(node: Node): void {
    let el = node as HTMLElement;
    this.newValue = (el.style as any)[this.prop];
    (el.style as any)[this.prop] = this.oldValue;
  }
  redo(node: Node): void {
    let el = node as HTMLElement;
    this.oldValue = (el.style as any)[this.prop]!;
    (el.style as any)[this.prop] = this.newValue;
  }
}
