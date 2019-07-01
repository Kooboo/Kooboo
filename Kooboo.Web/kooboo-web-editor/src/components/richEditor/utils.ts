import { Editor, Settings, EditorManager } from "tinymce";
import {
  STANDARD_Z_INDEX,
  KOOBOO_GUID,
  ACTION_TYPE,
  EMPTY_COMMENT,
  EDITOR_TYPE
} from "../../common/constants";
import { lang } from "../../common/lang";
import context from "../../common/context";
import { markDirty, setGuid } from "../../common/koobooUtils";
import { Operation } from "../../models/Operation";
import { getAllElement } from "../../dom/utils";
import { delay } from "../../common/utils";
import { getEditComment } from "../floatMenu/utils";
import moveIcon from "@/assets/icons/drag-move--fill.svg";

export async function impoveEditorUI(editor: Editor) {
  let container = editor.getContainer();
  if (container instanceof HTMLElement) {
    container.style.zIndex = STANDARD_Z_INDEX + 1 + "";
    await delay(100);
    if (container.nextElementSibling instanceof HTMLElement) {
      container.nextElementSibling.style.zIndex = STANDARD_Z_INDEX + 2 + "";
    }
    let toolbar = container
      .getElementsByClassName("tox-toolbar")
      .item(0) as HTMLElement;
    var moveBtn = document.createElement("img");
    moveBtn.draggable = true;
    moveBtn.style.cursor = "move";
    moveBtn.src = moveIcon;
    moveBtn.style.height = "28px";
    moveBtn.style.margin = "4px";
    toolbar.insertBefore(moveBtn, toolbar.children.item(0));
    container.draggable = true;
    container.ondrag = e => {
      if (!(container instanceof HTMLElement)) return;
      container.style.position = "fixed";
      if (e.x == 0 || e.y == 0) return;
      container.style.top = e.y - 15 + "px";
      container.style.left = e.x - 15 + "px";
    };
  }
}

export function setLang(settings: Settings) {
  if (lang == "zh") {
    settings.language = "zh_CN";
    settings.language_url = `_Admin\\Scripts\\kooboo-web-editor\\${
      settings.language
    }.js`;
  }
}

export function save_oncancelcallback(e: Editor) {
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
  if (startContent != element.innerHTML) {
    let dirtyEl = args.closeParent ? args.closeParent : element;
    markDirty(dirtyEl, true);
    let koobooId = args.parentKoobooId ? args.parentKoobooId : args.koobooId;

    let commit = args.closeParent
      ? args.closeParent.innerHTML
      : element.innerHTML;

    setGuid(element);

    let operation = new Operation(
      element.getAttribute(KOOBOO_GUID)!,
      startContent,
      element.innerHTML,
      getEditComment(args.koobooComments)!,
      koobooId,
      ACTION_TYPE.update,
      commit,
      EDITOR_TYPE.dom
    );
    context.operationManager.add(operation);
  }

  context.editing = false;
}

export function onBlur() {
  return false;
}

export function onSetContent(e: any) {
  e.target._content = e.content;
  var targetElm = e.target.targetElm as HTMLElement;
  for (const element of getAllElement(targetElm, true)) {
    if (
      (element.tagName.toLowerCase() == "i" ||
        element.tagName.toLowerCase() == "a") &&
      element.innerHTML.indexOf(EMPTY_COMMENT) == -1
    ) {
      element.innerHTML += EMPTY_COMMENT;
    }
  }
}

export function onRemove(e: any) {
  let element = e.target.getElement();
  element._tinymceeditor = null;

  if (!element._isRelative) {
    element.style.position = "";
  }

  if (element instanceof HTMLElement) {
    if (element.id.startsWith("mce_")) element.removeAttribute("id");
    if (element.getAttribute("style") == "") element.removeAttribute("style");
  }

  if (e.target._onremove) e.target._onremove();

  EditorManager.editors.forEach(i => {
    i.remove();
  });
}

export function onKeyDown(e: KeyboardEvent) {
  var targetElm = e.target as HTMLElement;
  if (e.code == "Backspace" && targetElm.innerHTML == EMPTY_COMMENT)
    return false;
}

export function onBeforeSetContent(e: any) {
  //fix tinymce known issue https://github.com/tinymce/tinymce/issues/2453
  var targetElm = e.target.targetElm as HTMLElement;
  if (targetElm.tagName.toLowerCase() == "li") {
    if (targetElm.children.length == 0) {
      e.content = targetElm.textContent;
    } else if (e.content === 0) {
      e.content = targetElm.innerHTML;
    }
  }

  e.format = "raw";
}

export function getToolbar(el: HTMLElement) {
  let items =
    "save cancel | undo redo | bold italic forecolor fontselect fontsizeselect | image ";
  if (el.tagName.toLowerCase() != "a") {
    items += "| link unlink";
  }

  return items;
}
