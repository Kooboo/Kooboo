import context from "../../context";
import { isSkipHover } from "../../common/dom";
import { FloatMenu } from "./FloatMenu";

let floatMenu: FloatMenu;

export function registerMenu(document: Document) {
  if (floatMenu) return;
  floatMenu = new FloatMenu(document);

  context.domChangeEvent.addEventListener(e => {
    floatMenu.update(e.mouseEvent.pageX, e.mouseEvent.pageY);
  });

  context.hoverDomEvent.addEventListener(() => floatMenu.clear());

  context.floatMenuClickEvent.addEventListener(() =>
    setTimeout(() => floatMenu.clear(), 300)
  );
}
