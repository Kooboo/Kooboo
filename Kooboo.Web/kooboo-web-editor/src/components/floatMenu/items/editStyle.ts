import { TEXT } from "@/common/lang";
import { createItem, MenuItem } from "../basic";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getEditComment, getMenu, getForm, getHtmlBlock } from "../utils";
import { createStyleEditor } from "@/components/styleEditor";
import { isBody } from "@/dom/utils";
import { setGuid, clearKoobooInfo } from "@/kooboo/utils";
import { KOOBOO_GUID, ACTION_TYPE, EDITOR_TYPE } from "@/common/constants";

export function createEditStyleItem(): MenuItem {
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
    const startContent = args.element.getAttribute("style");
    setGuid(args.element);
    try {
      await createStyleEditor(args.element);
      // let operation = new Operation(
      //   args.element!.getAttribute(KOOBOO_GUID)!,
      //   startContent!,
      //   args.element.getAttribute("style")!,
      //   getEditComment(args.koobooComments)!,
      //   args.koobooId,
      //   ACTION_TYPE.update,
      //   args.element.getAttribute("style")!,
      //   EDITOR_TYPE.style
      // );
      // context.operationManager.add(operation);
    } catch (error) {
      if (startContent) args.element.setAttribute("style", startContent);
    }
  });

  return { el, update };
}
