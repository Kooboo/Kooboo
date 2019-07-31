import context from "@/common/context";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { delay } from "@/common/utils";
import { createMenu } from "./menu";
import { emitHoverEvent, emitSelectedEvent } from "@/dom/events";

export function createFloatMenu() {
  const { container, hidden, update } = createMenu();
  container.onmouseenter = () => (context.floatMenuClosing = false);

  context.domChangeEvent.addEventListener(e => {
    if (!context.lastMouseEventArg.isTrusted) return;
    context.floatMenuClosing = false;
    update(context.lastMouseEventArg!.pageX, context.lastMouseEventArg!.pageY);
  });

  context.hoverDomEvent.addEventListener(async () => {
    context.floatMenuClosing = true;
    await delay(200);
    if (context.floatMenuClosing) hidden();
  });

  context.floatMenuClickEvent.addEventListener(async e => {
    if(e==MenuActions.convert) return;
    if (e == MenuActions.close) {
      hidden();
    } else if (e != MenuActions.expand) {
      await delay(100);
      hidden();
    }
  });

  context.floatMenuClickEvent.addEventListener(async e => {
    if (context.editing || e != MenuActions.expand) return;
    let el = context.lastHoverDomEventArgs!.closeElement;
    if (!el || !el.parentElement) return;
    emitHoverEvent(el.parentElement);
    emitSelectedEvent();
  });

  return container;
}
