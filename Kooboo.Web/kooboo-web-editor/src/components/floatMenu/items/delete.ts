import { MenuItem, createItem } from "../basic";
import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { KOOBOO_GUID, ACTION_TYPE, EDITOR_TYPE } from "@/common/constants";
import { SelectedDomEventArgs } from "@/events/SelectedDomEvent";
import {
  setGuid,
  markDirty,
  cleanKoobooInfo,
  isDynamicContent
} from "@/common/koobooInfo";
import { Operation } from "@/models/Operation";
import { getEditComment } from "../utils";
import { isBody } from "@/dom/utils";

export function createDeleteItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.DELETE, MenuActions.delete);
  const update = () => {
    let visiable = true;
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) visiable = false;
    if (!args.parentKoobooId || !args.closeParent) visiable = false;
    if (!getEditComment(args.koobooComments)) visiable = false;
    if (isDynamicContent(args.element)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", () => {
    let args = context.lastSelectedDomEventArgs;
    if (args.closeParent) {
      updateDom(args);
    } else if (args.element.parentElement) {
      deleteDom(args);
    }
  });

  return {
    el,
    update
  };
}

const updateDom = (args: SelectedDomEventArgs) => {
  setGuid(args.closeParent!);
  let startContent = args.closeParent!.innerHTML;
  args.element.parentElement!.removeChild(args.element);
  markDirty(args.closeParent!);
  let endContent = args.closeParent!.innerHTML;
  let operation = new Operation(
    args.closeParent!.getAttribute(KOOBOO_GUID)!,
    startContent,
    endContent,
    args.koobooComments[0],
    args.parentKoobooId,
    ACTION_TYPE.update,
    cleanKoobooInfo(args.closeParent!.innerHTML),
    EDITOR_TYPE.dom
  );
  context.operationManager.add(operation);
};

const deleteDom = (args: SelectedDomEventArgs) => {
  let parentElement = args.element.parentElement!;
  setGuid(parentElement);
  let startContent = parentElement.innerHTML;
  parentElement.removeChild(args.element);
  let endContent = parentElement.innerHTML;
  let operation = new Operation(
    parentElement.getAttribute(KOOBOO_GUID)!,
    startContent,
    endContent,
    getEditComment(args.koobooComments)!,
    args.koobooId,
    ACTION_TYPE.delete,
    "",
    EDITOR_TYPE.dom
  );
  context.operationManager.add(operation);
};
