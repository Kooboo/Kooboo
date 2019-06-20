import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { MenuItem, createItem } from "../basic";
import { isDynamicContent } from "@/common/koobooInfo";
import { getEditComment, isInMenu, isInForm, isHtmlBlock } from "../utils";
import { isBody } from "@/dom/utils";

export function createEditItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT, MenuActions.edit);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (isInMenu(args.koobooComments)) visiable = false;
    if (isInForm(args.koobooComments)) visiable = false;
    if (isHtmlBlock(args.koobooComments)) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    var reExcept = /^img|button|input|textarea|br$/i;
    let el = args.element;
    if (reExcept.test(el.tagName)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    console.log(args, visiable);
    setVisiable(visiable);
  };

  return { el, update };
}
