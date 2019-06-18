import context from "../../context";
import { FloatMenu } from "./FloatMenu";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { getKoobooInfo, getCloseElement } from "../../common/koobooInfo";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";
import { HoverDomEventArgs } from "../../events/HoverDomEvent";
import { delay } from "../../common/utils";

let floatMenu: FloatMenu;

export function registerMenu(document: Document) {
  if (floatMenu) return;
  floatMenu = new FloatMenu(document);

  context.domChangeEvent.addEventListener(e => {
    floatMenu.update(
      context.lastMouseEventArg!.pageX,
      context.lastMouseEventArg!.pageY
    );
  });

  context.hoverDomEvent.addEventListener(() => floatMenu.clear());

  context.floatMenuClickEvent.addEventListener(async e => {
    if (e == MenuActions.close) {
      floatMenu.clear();
    } else if (e != MenuActions.expand) {
      await delay(100);
      floatMenu.clear();
    }
  });

  context.floatMenuClickEvent.addEventListener(async e => {
    if (context.editing || e != MenuActions.expand) return;
    let el = context.lastHoverDomEventArgs!.closeElement;
    if (!el || !el.parentElement) return;
    let closeElement = getCloseElement(el.parentElement);
    if (closeElement == null) return;

    context.hoverDomEvent.emit(
      new HoverDomEventArgs(el.parentElement, closeElement)
    );

    let { comments, koobooId, closeParent, parentKoobooId } = getKoobooInfo(el);
    if (comments.length == 0) return;
    var args = new SelectedDomEventArgs(
      closeElement,
      koobooId,
      closeParent,
      parentKoobooId,
      comments
    );
    context.domChangeEvent.emit(args);
  });
}
