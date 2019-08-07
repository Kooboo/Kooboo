import { Unit } from "./Unit";

export class CssUnit extends Unit {
  constructor(public oldValue: string, public prop: string, public rule: CSSStyleRule) {
    super(oldValue);
  }

  undo(node: Node): void {
    this.newValue = this.rule.style.getPropertyValue(this.prop);
    this.rule.style.setProperty(this.prop, this.oldValue);
  }

  redo(node: Node): void {
    this.oldValue = this.rule.style.getPropertyValue(this.prop);
    this.rule.style.setProperty(this.prop, this.newValue);
  }
}
