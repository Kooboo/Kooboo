import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { MenuItem, createItem } from "../basic";
import { isDynamicContent } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { setInlineEditor } from "@/components/richEditor";
import { getMenuComment, getFormComment, getHtmlBlockComment, getEditComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";

export function createEditItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT, MenuActions.edit);
  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (isBody(args.element)) return setVisiable(false);
    if (getMenuComment(comments)) return setVisiable(false);
    if (getFormComment(comments)) return setVisiable(false);
    if (!getEditComment(comments)) return setVisiable(false);
    var reExcept = /^img|button|input|textarea|br|hr$/i;
    let el = args.element;
    if (reExcept.test(el.tagName)) return setVisiable(false);
    if (isDynamicContent(args.element)) return setVisiable(false);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    setInlineEditor(args.element);
  });

  return { el, update };
}