import context from "../../context";
import { isSkipHover } from "../../dom/domAnalyze";
import { FloatMenu } from "./FloatMenu";

let floatMenu: FloatMenu;

export function registerMenu(document: Document) {
  if (floatMenu) return;
  floatMenu = new FloatMenu(document);

  document.body.addEventListener("click", e => {
    e.preventDefault();
    e.stopPropagation();
    if (context.editing || isSkipHover(e)) return;
    if (context.lastSelectedDomEventArgs) {
      floatMenu.update(e.pageX, e.pageY);
    }
  });

  context.domChangeEvent.addEventListener(e => floatMenu.clear());

  context.floatMenuClickEvent.addEventListener(e => floatMenu.clear());
}
