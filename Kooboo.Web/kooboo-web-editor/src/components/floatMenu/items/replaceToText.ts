import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment } from "../utils";
import { isDynamicContent, setGuid, getCloseElement } from "@/kooboo/utils";
import { setInlineEditor } from "@/components/richEditor";
import { KOOBOO_ID, KOOBOO_DIRTY } from "@/common/constants";
import { emitSelectedEvent, emitHoverEvent } from "@/dom/events";

export function createReplaceToTextItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.REPLACE_TO_TEXT, MenuActions.replaceToText);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) visiable = false;
    let closeParent = getCloseElement(args.element.parentElement!)!;
    let koobooId = closeParent.getAttribute(KOOBOO_ID);
    if (!closeParent || !koobooId) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let closeParent = getCloseElement(args.element.parentElement!)!;
    let element = args.cleanElement ? args.cleanElement : closeParent;
    setGuid(element);
    let startContent = element.innerHTML;
    try {
      let text = document.createElement("p");
      let style = getComputedStyle(args.element);
      text.setAttribute(KOOBOO_ID, args.koobooId!);
      text.setAttribute(KOOBOO_DIRTY, "");
      text.style.width = style.width;
      text.style.height = style.height;
      text.style.display = style.display;
      args.element.parentElement!.replaceChild(text, args.element);
      emitHoverEvent(text);
      emitSelectedEvent(text);
      await setInlineEditor(text);
    } catch (error) {
      element.innerHTML = startContent;
    }
  });

  return { el, update };
}
