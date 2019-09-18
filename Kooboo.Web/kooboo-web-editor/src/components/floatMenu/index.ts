import context from "@/common/context";
import { delay } from "@/common/utils";
import { createMenu } from "./menu";

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

  return container;
}
