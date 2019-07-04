import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getEditComment } from "../utils";
import { isDynamicContent, setGuid } from "@/kooboo/utils";
import { setInlineEditor } from "@/components/richEditor";
import { KOOBOO_ID } from "@/common/constants";
import { emitSelectedEvent, emitHoverEvent } from "@/dom/events";

export function createReplaceToTextItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.REPLACE_TO_TEXT, MenuActions.replaceToText);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (!isImg(args.element)) visiable = false;
    if (!args.closeParent || !args.parentKoobooId) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    if (!args.closeParent) return false;
    setGuid(args.closeParent);
    let startContent = args.closeParent.innerHTML;
    try {
      let text = document.createElement("p");
      let style = getComputedStyle(args.element);
      text.setAttribute(KOOBOO_ID, args.koobooId!);
      text.style.width = style.width;
      text.style.height = style.height;
      text.style.display = style.display;
      args.element.parentElement!.replaceChild(text, args.element);
      emitHoverEvent(text);
      emitSelectedEvent(text);
      await setInlineEditor(text);
    } catch (error) {
      args.closeParent.innerHTML = startContent;
    }
  });

  return { el, update };
}
