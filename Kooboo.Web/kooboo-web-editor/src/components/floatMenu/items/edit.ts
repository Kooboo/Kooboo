import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { MenuItem, createItem } from "../basic";
import { isDynamicContent } from "@/kooboo/koobooUtils";
import { getEditComment, getMenu, getForm, getHtmlBlock } from "../utils";
import { isBody } from "@/dom/utils";

export function createEditItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT, MenuActions.edit);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (getMenu(args.koobooComments)) visiable = false;
    if (getForm(args.koobooComments)) visiable = false;
    if (getHtmlBlock(args.koobooComments)) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    var reExcept = /^img|button|input|textarea|br|hr$/i;
    let el = args.element;
    if (reExcept.test(el.tagName)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  return { el, update };
}
