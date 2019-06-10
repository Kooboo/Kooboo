import { TEXT } from "../../../lang";

export function createTitle(document: Document) {
  const el = document.createElement("div");
  el.style.padding = "5px 10px";
  el.style.color = "#fff";
  el.style.backgroundColor = "rgb(2, 154, 214)";
  const text = document.createElement("span");
  text.innerText = TEXT.MENU;
  el.appendChild(text);
  return el;
}
