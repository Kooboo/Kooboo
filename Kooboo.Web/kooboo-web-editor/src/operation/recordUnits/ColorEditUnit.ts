import { Unit } from "./Unit";
import EditElement from "@/components/colorEditor/EditElement";

export class ColorEditUnit extends Unit {
  constructor(public oldValue: string, protected editElement: EditElement) {
    super(oldValue);
  }

  undo(node: Node): void {
    this.editElement.resetElement(node as HTMLElement);
    this.editElement.cancel();
  }

  redo(node: Node): void {
    this.editElement.resetElement(node as HTMLElement);
    this.editElement.ok();
  }
}
