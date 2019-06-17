import closeIcon from "../../assets/icons/guanbi.svg";
import expandIcon from "../../assets/icons/fangda.svg";
import { TEXT } from "../../lang";
import context from "../../context";
import { MenuActions } from "../../events/FloatMenuClickEvent";

export function createTitle(document: Document) {
  const el = document.createElement("div");
  el.style.padding = "5px 10px";
  el.style.color = "#fff";
  el.style.backgroundColor = "rgb(2, 154, 214)";
  const text = document.createElement("span");
  text.innerText = TEXT.MENU;
  el.appendChild(text);

  const closeBtn = document.createElement("img");
  closeBtn.src = closeIcon;
  closeBtn.style.height = "16px";
  closeBtn.style.cssFloat = "right";
  closeBtn.onclick = () => context.floatMenuClickEvent.emit(MenuActions.close);
  el.appendChild(closeBtn);

  const expandBtn = document.createElement("img");
  expandBtn.src = expandIcon;
  expandBtn.style.height = "16px";
  expandBtn.style.cssFloat = "right";
  expandBtn.style.marginRight = "8px";
  expandBtn.onclick = () =>
    context.floatMenuClickEvent.emit(MenuActions.expand);
  el.appendChild(expandBtn);
  return el;
}
