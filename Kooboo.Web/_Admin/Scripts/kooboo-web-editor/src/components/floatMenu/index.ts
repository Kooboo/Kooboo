import context from "../../context";
import { FloatMenu } from "./FloatMenu";
import { MenuActions } from "../../events/FloatMenuClickEvent";
import { getKoobooInfo, getCloseElement } from "../../common/koobooInfo";
import { SelectedDomEventArgs } from "../../events/SelectedDomEvent";
import { HoverDomEventArgs } from "../../events/HoverDomEvent";

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

  context.floatMenuClickEvent.addEventListener(e => {
    if (e == MenuActions.close) {
      floatMenu.clear();
    } else if (e != MenuActions.expand) {
      setTimeout(() => floatMenu.clear(), 100);
    }
  });

  context.floatMenuClickEvent.addEventListener(e => {
    if (e != MenuActions.expand) return;
    let el = context.lastHoverDomEventArgs!.closeElement!.parentElement;
    if (!el) return;
    let closeElement = getCloseElement(el);
    if (closeElement == null) return;
    context.hoverDomEvent.emit(new HoverDomEventArgs(el, closeElement));
    let { comments, koobooId, closeParent, parentKoobooId } = getKoobooInfo(
      context.lastHoverDomEventArgs!.closeElement!
    );

    if (comments.length == 0) return;

    var args = new SelectedDomEventArgs(
      context.lastHoverDomEventArgs!.closeElement!,
      koobooId,
      closeParent,
      parentKoobooId,
      comments
    );
    context.domChangeEvent.emit(args);
  });
}
