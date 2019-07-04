import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getHtmlBlock, hasOperation } from "../utils";
import { reload } from "@/dom/utils";
import { editHtmlBlock } from "@/kooboo/outsideInterfaces";

export function createEditHtmlBlockItem(): MenuItem {
  const { el, setVisiable, setReadonly } = createItem(TEXT.EDIT_HTML_BLOCK, MenuActions.editHtmlBlock);

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;

    if (!getHtmlBlock(args.koobooComments)) visiable = false;
    if (hasOperation(context.operationManager)) {
      setReadonly();
    }

    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let nameorid = args.koobooComments[0].nameorid;
    await editHtmlBlock(nameorid!);
    reload();
  });

  return { el, update };
}
