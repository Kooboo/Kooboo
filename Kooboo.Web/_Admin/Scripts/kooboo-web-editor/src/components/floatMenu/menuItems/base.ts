import context from "../../../context";
import { MenuActions } from "../../../events/FloatMenuClickEvent";

export abstract class BaseItem {
  abstract text: string;
  abstract type: MenuActions;
  constructor(readonly document: Document) {}
  abstract canShow(): boolean;
  get el() {
    let el = this.document.createElement("div");
    el.style.padding = "5px 10px";
    el.style.color = "#000";
    el.style.borderBottom = "1px solid #ddd";
    el.innerHTML = this.text;
    el.style.visibility = this.canShow() ? "visible" : "hidden";
    el.onclick = () => context.floatMenuClickEvent.emit(this.type);
    return el;
  }
}
