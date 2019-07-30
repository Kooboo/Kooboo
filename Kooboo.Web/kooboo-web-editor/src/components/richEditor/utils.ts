import { Editor, Settings, EditorManager } from "tinymce";
import { STANDARD_Z_INDEX, EMPTY_COMMENT, OBJECT_TYPE } from "../../common/constants";
import { lang } from "../../common/lang";
import context from "../../common/context";
import { markDirty, setGuid, clearKoobooInfo, getCleanParent, getWrapDom, isDirty } from "../../kooboo/utils";
import { getAllElement, isTextArea } from "../../dom/utils";
import { delay } from "../../common/utils";
import moveIcon from "@/assets/icons/drag-move--fill.svg";
import { operationRecord } from "@/operation/Record";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { getEditComment, getHtmlBlockComment } from "../floatMenu/utils";
import { Log } from "@/operation/recordLogs/Log";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { LabelLog } from "@/operation/recordLogs/LabelLog";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { HtmlblockLog } from "@/operation/recordLogs/HtmlblockLog";
import { createDiv, createImg } from "@/dom/element";

export async function impoveEditorUI(editor: Editor) {
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
  e.setContent((e as any)._content);
  e.save = () => ""; //fix loss element when tinymce editor removed
  e.remove();
  context.editing = false;
  callBack();
}

export function save_onsavecallback(e: Editor, callBack: () => void) {
  let args = context.lastSelectedDomEventArgs;
  if (!args) return;
  let startContent = (e as any)._content;
  let isRelpace = (e as any)._isReplace;
  let element = e.getElement() as HTMLElement;
  e.save = () => ""; //fix loss element when tinymce editor removed
  e.remove();

  let clearContent = (c: string) => clearKoobooInfo(c).replace(/\s/g, "");

  if (clearContent(startContent) != clearContent(element.innerHTML)) {
    let { koobooId, parent } = getCleanParent(args.element);
    let comments = KoobooComment.getComments(args.element);
    let dirtyEl = parent && isDirty(args.element) ? parent : element;
    markDirty(dirtyEl);
    koobooId = koobooId && isDirty(args.element) ? koobooId : args.koobooId;
    let guid = setGuid(isRelpace ? parent! : element);
    let units = [new InnerHtmlUnit(startContent)];
    let htmlBlockcomment = getHtmlBlockComment(comments);
    let comment = getEditComment(comments)!;
    let log: Log;

    if (comment.objecttype == OBJECT_TYPE.content) {
      log = ContentLog.createUpdate(comment.nameorid!, comment.fieldname!, clearKoobooInfo(element.innerHTML));
    } else if (comment.objecttype == OBJECT_TYPE.Label) {
      log = LabelLog.createUpdate(comment.bindingvalue!, clearKoobooInfo(element.innerHTML));
    } else if (htmlBlockcomment) {
      let { nodes } = getWrapDom(element, OBJECT_TYPE.htmlblock);
      let temp = createDiv();
      nodes.forEach(i => temp.appendChild(i.cloneNode(true)));
      log = HtmlblockLog.createUpdate(htmlBlockcomment.nameorid!, clearKoobooInfo(temp.innerHTML));
    } else {
      log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(dirtyEl.innerHTML), koobooId!, comment.objecttype!);
    }

    let operation = new operationRecord(units, [log], guid);
    context.operationManager.add(operation);
  }

  context.editing = false;
  callBack();
}

export function onBlur() {
  return false;
}

export function onSetContent(e: any) {
  e.target._content = e.content;
  var targetElm = e.target.targetElm as HTMLElement;
  for (const element of getAllElement(targetElm, true)) {
    if (element.innerHTML.indexOf(EMPTY_COMMENT) == -1 && !isTextArea(element)) {
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

  EditorManager.editors.forEach(i => {
    i.remove();
  });
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
