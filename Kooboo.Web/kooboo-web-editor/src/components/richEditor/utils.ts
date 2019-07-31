import { Editor, Settings } from "tinymce";
import { STANDARD_Z_INDEX, EMPTY_COMMENT } from "../../common/constants";
import { lang } from "../../common/lang";
import context from "../../common/context";
import { getAllElement, isTextArea } from "../../dom/utils";
import { delay } from "../../common/utils";
import moveIcon from "@/assets/icons/drag-move--fill.svg";
import { createImg } from "@/dom/element";

export async function impoveEditorUI(editor: Editor) {
  editor.focus(false);
  let container = editor.getContainer();
  if (container instanceof HTMLElement) {
    container.style.zIndex = STANDARD_Z_INDEX + 1 + "";
    container.style.width = "auto";
    await delay(100);
    if (container.nextElementSibling instanceof HTMLElement) {
      container.nextElementSibling.style.zIndex = STANDARD_Z_INDEX + 2 + "";
    }
    let toolbar = container.getElementsByClassName("tox-toolbar").item(0) as HTMLElement;
    var moveBtn = createImg();
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
    settings.language_url = `${location.origin}\\_Admin\\Scripts\\kooboo-web-editor\\${settings.language}.js`;
  }
}

export function save_oncancelcallback(e: Editor, callBack: () => void) {
  e.save = () => ""; //fix loss element when tinymce editor removed
  e.remove();
  callBack();
}

export function save_onsavecallback(e: Editor, callBack: () => void) {
  e.save = () => ""; //fix loss element when tinymce editor removed
  e.remove();
  callBack();
}

export function onBlur() {
  return false;
}

export function onSetContent(e: any) {
  var targetElm = e.target.targetElm as HTMLElement;
  for (const element of getAllElement(targetElm, true)) {
    if (element.innerHTML.indexOf(EMPTY_COMMENT) == -1 && !isTextArea(element)) {
      element.innerHTML += EMPTY_COMMENT;
    }
  }
}

export function onRemove(e: any) {
  let element = e.target.getElement();
  if (!element._isRelative) {
    element.style.position = "";
  }

  if (element instanceof HTMLElement) {
    if (element.id.startsWith("mce_")) element.removeAttribute("id");
    if (element.getAttribute("style") == "") element.removeAttribute("style");
  }
  context.editing = false;
}

export function onKeyDown(e: KeyboardEvent) {
  var targetElm = e.target as HTMLElement;
  if (e.code == "Backspace") {
    let content = targetElm.innerHTML;
    if (content.indexOf(EMPTY_COMMENT) == -1 && !isTextArea(targetElm)) targetElm.innerHTML += EMPTY_COMMENT;
  }
  if (e.code == "Backspace" && targetElm.innerHTML == EMPTY_COMMENT) return false;
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
  let items = "save cancel | undo redo | bold italic underline | forecolor backcolor | fontselect fontsizeselect | image ";
  if (el.tagName.toLowerCase() != "a") {
    items += "| link unlink";
  }

  return items;
}

export function initInstanceCallback(e: Editor) {
  context.editing = true;
  impoveEditorUI(e);
  context.closeEditingEvent.addEventListener(() => {
    e.execCommand("mcecancel");
    context.closeEditingEvent.handlers = [];
  });
}
