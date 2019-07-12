import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg } from "@/dom/utils";
import { getFirstComment, isViewComment } from "../utils";
import { isDynamicContent, setGuid, getCleanParent } from "@/kooboo/utils";
import { setInlineEditor } from "@/components/richEditor";
import { KOOBOO_ID, KOOBOO_DIRTY } from "@/common/constants";
import { emitSelectedEvent, emitHoverEvent } from "@/dom/events";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createReplaceToTextItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.REPLACE_TO_TEXT, MenuActions.replaceToText);
  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let firstComment = getFirstComment(comments);
    if (!firstComment || !isViewComment(firstComment)) setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent || !koobooId) setVisiable(false);
    if (!isImg(args.element)) setVisiable(false);
    if (isDynamicContent(args.element)) setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let { parent } = getCleanParent(args.element);
    let startContent = parent!.innerHTML;
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
      emitSelectedEvent();
      await setInlineEditor(text, startContent);
    } catch (error) {
      parent!.innerHTML = startContent;
    }
  });

  return { el, update };
}
