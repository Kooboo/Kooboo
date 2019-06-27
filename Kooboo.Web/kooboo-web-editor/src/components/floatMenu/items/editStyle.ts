import { TEXT } from "@/common/lang";
import { createItem } from "../basic";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getEditComment } from "../utils";
import { createStyleEditor } from "@/components/styleEditor";

export function createEditStyleItem() {
  const { el, setVisiable } = createItem(
    TEXT.EDIT_STYLE,
    MenuActions.editStyle
  );

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;

    if (!getEditComment(args.koobooComments)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comment = getEditComment(args.koobooComments);
    createStyleEditor(args.element);
    // reload();
  });

  return { el, update };
}
