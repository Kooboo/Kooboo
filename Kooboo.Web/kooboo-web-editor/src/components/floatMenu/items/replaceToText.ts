import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isImg, isInTable } from "@/dom/utils";
import { getRepeatComment, getViewComment, getEditComment, clearContent } from "../utils";
import { isDynamicContent, getCleanParent, getRelatedRepeatComment, clearKoobooInfo, setGuid } from "@/kooboo/utils";
import { setInlineEditor } from "@/components/richEditor";
import { KOOBOO_ID, KOOBOO_DIRTY } from "@/common/constants";
import { emitSelectedEvent, emitHoverEvent } from "@/dom/events";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createP } from "@/dom/element";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";

export function createReplaceToTextItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.REPLACE_TO_TEXT, MenuActions.replaceToText);
  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (getRepeatComment(comments)) return setVisiable(false);
    if (getRelatedRepeatComment(args.element)) return setVisiable(false);
    if (isInTable(args.element)) return setVisiable(false);
    if (!getViewComment(comments)) return setVisiable(false);
    let { koobooId, parent } = getCleanParent(args.element);
    if (!parent || !koobooId) return setVisiable(false);
    if (!isImg(args.element)) return setVisiable(false);
    if (isDynamicContent(parent)) return setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let { parent, koobooId } = getCleanParent(args.element);
    let startContent = parent!.innerHTML;
    let text = createP();
    let style = getComputedStyle(args.element);
    text.setAttribute(KOOBOO_ID, args.koobooId!);
    text.setAttribute(KOOBOO_DIRTY, "");
    text.style.width = style.width;
    text.style.height = style.height;
    text.style.display = "block";
    args.element.parentElement!.replaceChild(text, args.element);
    emitHoverEvent(text);
    emitSelectedEvent();

    const onSave = () => {
      if (clearContent(startContent) == clearContent(text.innerHTML)) return;
      let guid = setGuid(parent!);
      let comments = KoobooComment.getComments(parent!);
      let comment = getEditComment(comments)!;
      let unit = new InnerHtmlUnit(startContent);
      let log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(parent!.innerHTML), koobooId!, comment.objecttype!);
      let operation = new operationRecord([unit], [log], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      parent!.innerHTML = startContent;
    };

    await setInlineEditor({ selector: text, onSave, onCancel });
  });

  return { el, update };
}
