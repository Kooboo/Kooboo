import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isHtmlBlock } from "../utils";
import { OBJECT_TYPE } from "@/common/constants";
import { editHtmlBlock } from "@/common/outsideInterfaces";

export function createEditHtmlBlockItem(): MenuItem {
  const { el, setVisiable } = createItem(
    TEXT.EDIT_HTML_BLOCK,
    MenuActions.editHtmlBlock
  );

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = false;

    if (
      args.koobooComments[0] &&
      args.koobooComments[0].objecttype == OBJECT_TYPE.htmlblock
    ) {
      visiable = true;
    }

    setVisiable(visiable);
  };

  el.addEventListener("click", e => {
    let args = context.lastSelectedDomEventArgs;
    let nameorid = args.koobooComments[0].nameorid;
    editHtmlBlock(nameorid!, result => (args.element.outerHTML = result));
  });

  return { el, update };
}
