import { Editor } from "tinymce";
import context from "../../context";
import { ACTION_TYPE, KOOBOO_DIRTY, KOOBOO_GUID } from "../../constants";
import { Operation } from "../../models/Operation";
import { getAllElement } from "../../common/dom";

export function save_oncancelcallback(e: Editor) {
  let element = e.getElement() as HTMLElement;
  if (!(element as any)._isRelative) {
    element.style.position = "";
  }
  e.setContent((e as any)._content);
  e.remove();
  context.editing = false;
}

export function save_onsavecallback(e: Editor) {
  let args = context.lastSelectedDomEventArgs;
  if (!args) return;
  let startContent = (e as any)._content;
  let element = e.getElement() as HTMLElement;
  e.remove();

  if (!(element as any)._isRelative) {
    element.style.position = "";
  }

  if (startContent != element.innerHTML) {
    for (const i of getAllElement(element, true)) {
      i.setAttribute(KOOBOO_DIRTY, "");
    }

    let koobooId = args.parentKoobooId ? args.parentKoobooId : args.koobooId;
    let commit = args.closeParent
      ? args.closeParent.innerHTML
      : element.innerHTML;

    console.log(commit);

    if (!element.hasAttribute(KOOBOO_GUID)) {
      element.setAttribute(KOOBOO_GUID, Math.random().toString());
    }

    let operation = new Operation(
      element.getAttribute(KOOBOO_GUID)!,
      startContent,
      element.innerHTML,
      args.editComment!,
      koobooId,
      ACTION_TYPE.update,
      commit
    );
    context.operationManager.add(operation);
  }

  context.editing = false;
}
