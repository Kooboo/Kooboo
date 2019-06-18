import { MenuActions } from "../../events/FloatMenuClickEvent";
import context from "../../context";

export abstract class BaseItem {
  abstract text: string;
  abstract type: MenuActions;
  constructor(readonly document: Document) {}
  abstract canShow(): boolean;
  click(e: MouseEvent) {}
  get el() {
    if (!this.canShow()) return null;
    let el = this.document.createElement("div");
    el.style.padding = "5px 10px";
    el.style.color = "#000";
    el.style.borderBottom = "1px solid #ddd";
    el.style.cursor = "default";
    el.innerHTML = this.text;

    el.onclick = e => {
      this.click(e);
      context.floatMenuClickEvent.emit(this.type);
    };

    return el;
  }
}
