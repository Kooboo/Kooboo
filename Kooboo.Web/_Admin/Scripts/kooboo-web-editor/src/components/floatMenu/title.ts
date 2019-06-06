import { TEXT } from "../../lang";

export function createTitle(document: Document) {
  const el = document.createElement("div");
  el.style.height = "20px";
  el.style.backgroundColor = "rgb(2, 154, 214)";
  el.innerHTML = TEXT.MENU;
  return el;
}
