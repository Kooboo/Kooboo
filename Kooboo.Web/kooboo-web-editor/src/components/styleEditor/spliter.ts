import { createDiv } from "@/dom/element";

export function createSpliter(text: string) {
  let el = createDiv();
  el.innerText = text;
  el.style.padding = "5px 0";
  el.style.margin = "5px 0";
  el.style.borderBottom = "1px solid #eee";
  return el;
}
