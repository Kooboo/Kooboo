import context from "../../common/context";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { getKoobooInfo, getCloseElement } from "../../common/koobooUtils";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";
import { HoverDomEventArgs } from "../../events/HoverDomEvent";
import { delay } from "../../common/utils";
import { createMenu } from "./menu";

export function createFloatMenu() {
  const menu = createMenu();
  context.domChangeEvent.addEventListener(e => {
    menu.update(
      context.lastMouseEventArg!.pageX,
      context.lastMouseEventArg!.pageY
    );
  });

  context.hoverDomEvent.addEventListener(() => menu.hidden());

  context.floatMenuClickEvent.addEventListener(async e => {
    if (e == MenuActions.close) {
      menu.hidden();
    } else if (e != MenuActions.expand) {
      await delay(100);
      menu.hidden();
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

  return menu.el;
}
