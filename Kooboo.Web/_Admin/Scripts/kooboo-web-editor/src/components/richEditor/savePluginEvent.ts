import { Editor } from "tinymce";
import context from "../../context";
import { ACTION_TYPE, OBJECT_TYPE } from "../../constants";
import { Operation } from "../../models/Operation";

export function save_oncancelcallback(e: Editor) {
  e.setContent((e as any)._content);
  e.remove();
  context.editing = false;
}

export function save_onsavecallback(e: Editor) {
  let args = context.lastSelectedDomEventArgs;
  if (!args) return;
  let startContent = (e as any)._content;
  let endContent = e.getContent();
  let element = e.getElement() as HTMLElement;
  e.remove();

  let comment = args.koobooComments.find(
    f =>
      f.objecttype != OBJECT_TYPE.contentrepeater &&
      f.objecttype != OBJECT_TYPE.attribute
  );

  let koobooId = args.parentKoobooId ? args.parentKoobooId : args.koobooId;
  let commit = args.closeParent ? args.closeParent.innerHTML : endContent;

  let operation = new Operation(
    element,
    startContent,
    endContent,
    comment!,
    koobooId,
    ACTION_TYPE.update,
    commit
  );
  context.operationManager.add(operation);
  context.editing = false;
}
