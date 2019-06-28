import { TEXT } from "@/common/lang";
import { createItem } from "../basic";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getEditComment, getMenu, getForm, getHtmlBlock } from "../utils";
import { createStyleEditor } from "@/components/styleEditor";
import { isBody } from "@/dom/utils";

export function createEditStyleItem() {
  const { el, setVisiable } = createItem(
    TEXT.EDIT_STYLE,
    MenuActions.editStyle
  );

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;
    if (isBody(args.element)) visiable = false;
    if (getMenu(args.koobooComments)) visiable = false;
    if (getForm(args.koobooComments)) visiable = false;
    if (getHtmlBlock(args.koobooComments)) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getEditComment(args.koobooComments);
    const startContent = args.element.getAttribute("style");
    try {
      await createStyleEditor(args.element);
    } catch (error) {
      if (startContent) args.element.setAttribute("style", startContent);
    }
  });

  return { el, update };
}
