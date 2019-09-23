import { Unit } from "./Unit";
import { CssColorGroup } from "@/dom/style";

export class CssUnit extends Unit {
  constructor(public oldValue: string, public groups: CssColorGroup[]) {
    super(oldValue);
  }

  undo(node: Node): void {
    for (const group of this.groups) {
      for (const color of group.cssColors) {
        if (color.inline) continue;
        color.cssStyleRule!.style.setProperty(color.prop.prop, color.value, color.important ? "important" : "");
      }
    }
  }

  redo(node: Node): void {
    for (const group of this.groups) {
      for (const color of group.cssColors) {
        if (color.inline || !color.newValue || color.newImportant === undefined) continue;
        color.cssStyleRule!.style.setProperty(color.prop.prop, color.newValue, color.newImportant ? "important" : "");
      }
    }
  }
}
