import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getHtmlBlock, hasOperation } from "../utils";
import { editHtmlBlock } from "@/common/outsideInterfaces";
import { reload } from "@/dom/utils";

export function createEditHtmlBlockItem(): MenuItem {
  const { el, setVisiable } = createItem(
    TEXT.EDIT_HTML_BLOCK,
    MenuActions.editHtmlBlock
  );

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;

    if (!getHtmlBlock(args.koobooComments)) visiable = false;
    if (hasOperation(context.operationManager)) visiable = false;

    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let nameorid = args.koobooComments[0].nameorid;
    let result = await editHtmlBlock(nameorid!);
    reload();
  });

  return { el, update };
}