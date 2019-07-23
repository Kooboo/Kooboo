import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";

export function createClickItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.CLICK, MenuActions.click);

  const update = () => {
    setVisiable(true);
  };

  el.addEventListener("click", () => {
    let element = context.lastHoverDomEventArgs.element;
    if ((element as any)._a) (element as any)._a.click();
    else element.click();
  });

  return {
    el,
    update
  };
}
