import context from "../../../context";
import { MenuActions } from "../../../events/FloatMenuClickEvent";
import { SelectedDomEventArgs } from "../../../events/SelectedDomEvent";

export abstract class BaseItem {
  abstract text: string;
  abstract type: MenuActions;
  constructor(
    readonly document: Document,
    readonly selectedDomEventArgs: SelectedDomEventArgs | undefined
  ) {}
  abstract canShow(
    selectedDomEventArgs: SelectedDomEventArgs | undefined
  ): boolean;
  get el() {
    let el = this.document.createElement("div");
    el.style.padding = "5px 10px";
    el.style.color = "#000";
    el.style.borderBottom = "1px solid #ddd";
    el.style.cursor = "default";
    el.innerHTML = this.text;

    el.style.display = this.canShow(this.selectedDomEventArgs)
      ? "block"
      : "none";

    el.onclick = () => context.floatMenuClickEvent.emit(this.type);
    return el;
  }
}
