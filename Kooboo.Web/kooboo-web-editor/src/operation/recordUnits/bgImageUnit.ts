import { Unit } from "./Unit";

export class BgImageUnit extends Unit {
  constructor(public oldValue: string, public style: CSSStyleDeclaration) {
    super(oldValue);
  }

  undo(node: Node): void {
    this.newValue = this.style.backgroundImage!;
    this.style.setProperty("background-image", this.oldValue, this.style.getPropertyPriority("background-image"));
  }

  redo(node: Node): void {
    this.oldValue = this.style.backgroundImage!;
    this.style.setProperty("background-image", this.newValue, this.style.getPropertyPriority("background-image"));
  }
}
