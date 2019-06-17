import context from "../../context";
import { FloatMenu } from "./FloatMenu";
import { MenuActions } from "../../events/FloatMenuClickEvent";

let floatMenu: FloatMenu;

export function registerMenu(document: Document) {
  if (floatMenu) return;
  floatMenu = new FloatMenu(document);

  context.domChangeEvent.addEventListener(e => {
    floatMenu.update(e.mouseEvent.pageX, e.mouseEvent.pageY);
  });

  context.hoverDomEvent.addEventListener(() => floatMenu.clear());

  context.floatMenuClickEvent.addEventListener(e => {
    if (e == MenuActions.close) {
      floatMenu.clear();
    } else {
      setTimeout(() => floatMenu.clear(), 100);
    }
  });
}
