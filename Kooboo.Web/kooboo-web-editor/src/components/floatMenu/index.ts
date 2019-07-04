import context from "@/common/context";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { delay } from "@/common/utils";
import { createMenu } from "./menu";
import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";

export function createFloatMenu() {
  const menu = createMenu();
  context.domChangeEvent.addEventListener(e => {
    menu.update(context.lastMouseEventArg!.pageX, context.lastMouseEventArg!.pageY);
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
    emitHoverEvent(el.parentElement);
    emitSelectedEvent(context.lastHoverDomEventArgs.closeElement);
  });

  return menu.el;
}
