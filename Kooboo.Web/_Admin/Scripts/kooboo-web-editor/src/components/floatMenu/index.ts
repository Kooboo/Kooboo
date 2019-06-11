import context from "../../context";
import { isSkipHover } from "../../helpers/domAnalyze";
import { FloatMenu } from "./FloatMenu";

let floatMenu: FloatMenu;

export function registerMenu(document: Document) {
  if (floatMenu) return;
  floatMenu = new FloatMenu(document);

  document.body.addEventListener("click", e => {
    e.preventDefault();
    e.stopPropagation();
    if (context.editing || isSkipHover(e)) return;
    console.log(context.lastSelectedDomEventArgs);
    if (context.lastSelectedDomEventArgs) {
      floatMenu.update(e.pageX, e.pageY);
    }
  });

  context.domChangeEvent.addEventListener(() => floatMenu.clear());

  context.floatMenuClickEvent.addEventListener(() =>
    setTimeout(() => floatMenu.clear(), 300)
  );
}
