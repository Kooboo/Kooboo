import { createItem, MenuItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { isBody } from "@/dom/utils";
import { getRepeat } from "../utils";
import { getWrapDom } from "@/kooboo/koobooUtils";
import { OBJECT_TYPE } from "@/common/constants";

export function createCopyRepeatItem(): MenuItem {
  const { el, setVisiable } = createItem(
    TEXT.COPY_REPEAT,
    MenuActions.copyRepeat
  );

  const update = () => {
    var visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (!getRepeat(args.koobooComments)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    let { nodes } = getWrapDom(args.element, OBJECT_TYPE.contentrepeater);
    if (!nodes || nodes.length == 0) return;
    let anchor: Node = nodes[nodes.length - 1];
    let parent = anchor.parentNode!;
    for (const node of nodes.reverse()) {
      let insertNode = node.cloneNode(true);
      parent.insertBefore(insertNode, anchor.nextSibling);
    }
  });

  return { el, update };
}
