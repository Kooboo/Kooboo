import tinymce from "tinymce-declaration";
import { STANDARD_Z_INDEX, EMPTY_COMMENT } from "../../common/constants";
import { lang } from "../../common/lang";
import context from "../../common/context";
import { getAllElement, isTextArea, getAllNode } from "../../dom/utils";
import { delay } from "../../common/utils";
import moveIcon from "@/assets/icons/drag-move--fill.svg";
import { createDiv } from "@/dom/element";
import { KoobooComment } from "@/kooboo/KoobooComment";

export async function impoveEditorUI(editor: tinymce.Editor) {
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
    var moveBtn = createDiv();
    moveBtn.draggable = true;
    moveBtn.style.cursor = "move";
    moveBtn.style.backgroundImage = `url(${moveIcon})`;
    moveBtn.style.backgroundPosition = `center`;
    moveBtn.style.backgroundRepeat = `no-repeat`;
    moveBtn.style.backgroundSize = `contain`;
    moveBtn.style.height = "26px";
    moveBtn.style.width = "26px";
    moveBtn.style.margin = "6px 0 6px 6px";
    toolbar.insertBefore(moveBtn, toolbar.children.item(0));
    container.draggable = true;
    const correctPositon = () => {
      let rect = container.getBoundingClientRect();
      if (rect.left < 0) (container as HTMLElement).style.left = "0px";
      if (rect.top < 0) (container as HTMLElement).style.top = "0px";
    };
    correctPositon();
    editor.on("MouseDown", correctPositon);
    editor.on("MouseUp", correctPositon);
    editor.on("KeyUp", correctPositon);
    container.ondrag = e => {
      if (!(container instanceof HTMLElement)) return;
      container.style.position = "fixed";
      if (e.x == 0 || e.y == 0) return;
      container.style.top = e.y - 15 + "px";
      container.style.left = e.x - 15 + "px";
    };
  }
}

export function setLang(settings: tinymce.Settings) {
  if (lang == "zh") {
    settings.language = "zh_CN";
    settings.language_url = `${location.origin}\\_Admin\\Scripts\\kooboo-web-editor\\${settings.language}.js`;
  }
}

export function onBlur() {
  return false;
}

export function onSetContent(e: any) {
  var targetElm = e.target.targetElm as HTMLElement;
  targetElm.style.position = (targetElm as any)._position;
  for (const element of getAllElement(targetElm, true)) {
    if (element.innerHTML.indexOf(EMPTY_COMMENT) == -1 && !isTextArea(element)) {
      element.innerHTML += EMPTY_COMMENT;
    }
  }
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

  for (let i = 0; i < targetElm.childNodes.length; i++) {
    const element = targetElm.childNodes[i];
    if (KoobooComment.isComment(element)) {
      (targetElm as any).kbComment = new KoobooComment(element);
      break;
    }
  }
}

export function getToolbar(el: HTMLElement) {
  let items = "save cancel | undo redo | bold italic underline | forecolor backcolor | fontselect fontsizeselect | image ";
  if (el.tagName.toLowerCase() != "a") {
    items += "| link unlink";
  }

  return items;
}

export function initInstanceCallback(e: tinymce.Editor) {
  context.editing = true;
  impoveEditorUI(e);
  context.closeEditingEvent.addEventListener(() => {
    e.execCommand("mcecancel");
    context.closeEditingEvent.handlers = [];
  });
}

export function savePluginCallback(e: tinymce.Editor, callBack: () => void) {
  e.save = () => ""; //fix loss element when tinymce editor removed
  e.remove();
  let element = e.getElement();
  if (element instanceof HTMLElement) {
    if (element.id.startsWith("mce_")) element.removeAttribute("id");
    if (element.getAttribute("style") == "") element.removeAttribute("style");
  }
  clearTinymceElements(document.body);
  context.editing = false;
  callBack();

  let kbComment = (element as any).kbComment as KoobooComment;
  if (kbComment && !element.innerHTML.trim().startsWith("<!--#kooboo")) {
    element.insertBefore(kbComment.ToComment(), element.firstChild);
  }
}

export function clearTinymceElements(root: HTMLElement) {
  let deleted = [];
  for (const i of getAllElement(root)) {
    if (i.classList.contains("mce-item-table")) i.classList.remove("mce-item-table");
    if (i.hasAttribute("data-mce-src")) i.removeAttribute("data-mce-src");
    if (i.hasAttribute("data-mce-bogus")) deleted.push(i);
  }
  deleted.forEach(i => i.parentElement && i.parentElement.removeChild(i));
}
