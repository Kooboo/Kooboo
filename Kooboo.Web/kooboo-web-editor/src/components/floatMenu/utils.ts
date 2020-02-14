import { OBJECT_TYPE, KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationManager } from "@/operation/Manager";
import { getAllNode } from "@/dom/utils";
import { setGuid, markDirty, clearKoobooInfo, getUnpollutedEl } from "@/kooboo/utils";
import { createImagePicker } from "../imagePicker";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { StyleUnit } from "@/operation/recordUnits/StyleUnit";
import { createLinkPicker } from "../linkPicker";
import { createDiv } from "@/dom/element";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";

export function getEditComment(comments: KoobooComment[]) {
  for (const i of comments) {
    if (i.source == "none") return;
    if (i.source && !i.source.startsWith("repeat") && !i.getValue("attribute")) return i;
  }
}

export function getRepeatItemId(comments: KoobooComment[]) {
  var repeatComment = comments.find(f => f.source == "repeatitem");
  if (!repeatComment) return;
  for (const commnet of comments) {
    var id = commnet.getValue("id");
    if (id) {
      let fullpathComment = commnet.getValue("fullpath");
      let path = repeatComment.getValue("path");
      if (fullpathComment && path && fullpathComment.startsWith(path)) return id;
    }
  }
}

export function getScopeComnent(comments: KoobooComment[]) {
  for (const i of comments) {
    if (i.source == "none") return;
    if (!i.getValue("scope")) return i;
  }
}

export function hasOperation(operationManager: operationManager) {
  return operationManager.previousRecords.length > 0;
}

export function changeNameOrId(node: Node, guid: string, oldGuid: string) {
  if (KoobooComment.isComment(node) && node.nodeValue!.indexOf(oldGuid) > -1) {
    node.nodeValue = node.nodeValue!.replace(/--id='.{36,50}?'/, `--id='${guid}'`);
  }
  if (node instanceof HTMLElement) {
    for (const iterator of getAllNode(node)) {
      changeNameOrId(iterator, guid, oldGuid);
    }
  }
}

export async function updateDomImage(element: HTMLImageElement) {
  let el = getUnpollutedEl(element)!;
  let parent = el == element ? el.parentElement! : el;
  let startContent = element.cloneNode(true) as HTMLImageElement;
  let comments = KoobooComment.getComments(element);
  let comment = getScopeComnent(comments)!;
  try {
    await createImagePicker(element as HTMLImageElement);
    if (startContent.outerHTML == element.outerHTML) return;
    let guid = setGuid(el);
    let value = clearKoobooInfo(parent.innerHTML);
    let oldSrc = startContent.getAttribute("src");
    let unit = new AttributeUnit(oldSrc!, "src");
    let log = [...comment.infos, kvInfo.value(value), kvInfo.koobooId(parent.getAttribute(KOOBOO_ID))];
    let record = new operationRecord([unit], [new Log(log)], guid);
    context.operationManager.add(record);
    return element.getAttribute("src")!;
  } catch (error) {
    element.setAttribute("src", startContent.getAttribute("src")!);
    element.setAttribute("style", startContent.getAttribute("style")!);
    element.title = startContent.title;
    element.alt = startContent.alt;
  }
}

export async function updateAttributeImage(element: HTMLImageElement, koobooId: string, comment: KoobooComment) {
  let startContent = element.cloneNode(true) as HTMLImageElement;

  let temp = createDiv();
  temp.appendChild(startContent);
  try {
    await createImagePicker(element);
    if (startContent.outerHTML == element.outerHTML) return;
    let guid = setGuid(element);

    let oldSrc = startContent.getAttribute("src");
    let widthImportant = element.style.getPropertyPriority("width");
    let heightImportant = element.style.getPropertyPriority("height");
    let units = [
      new AttributeUnit(startContent.title, "title"),
      new AttributeUnit(startContent.alt, "alt"),
      new AttributeUnit(oldSrc!, "src"),
      new StyleUnit(startContent.style.width!, "width", widthImportant),
      new StyleUnit(startContent.style.height!, "height", heightImportant)
    ];

    let newSrc = element.getAttribute("src")!;
    let logs = [
      DomLog.createUpdate(comment.nameorid!, element.title, koobooId, comment.objecttype!, "title"),
      DomLog.createUpdate(comment.nameorid!, element.alt, koobooId, comment.objecttype!, "alt"),
      DomLog.createUpdate(comment.nameorid!, newSrc, koobooId, comment.objecttype!, "src"),
      StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, element.style.width!, "width", koobooId, !!widthImportant),
      StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, element.style.height!, "height", koobooId, !!heightImportant)
    ];

    let record = new operationRecord(units, logs, guid);
    context.operationManager.add(record);
    return newSrc;
  } catch (error) {
    element.setAttribute("src", startContent.getAttribute("src")!);
    element.setAttribute("style", startContent.getAttribute("style")!);
    element.title = startContent.title;
    element.alt = startContent.alt;
  }
}

export async function updateDomLink(element: HTMLElement) {
  let el = getUnpollutedEl(element)!;
  let parent = el == element ? el.parentElement! : el;
  let comments = KoobooComment.getComments(element);
  let comment = getScopeComnent(comments)!;
  let href = element.getAttribute("href")!;

  try {
    let url = await createLinkPicker(href);
    element.setAttribute("href", url);
    let guid = setGuid(element);
    let value = clearKoobooInfo(parent.innerHTML);
    let unit = new AttributeUnit(href!, "href");
    let log = [...comment.infos, kvInfo.value(value), kvInfo.koobooId(parent.getAttribute(KOOBOO_ID))];
    let record = new operationRecord([unit], [new Log(log)], guid);
    context.operationManager.add(record);
    return url;
  } catch (error) {
    element.setAttribute("href", href);
  }
}

export async function updateAttributeLink(element: HTMLElement) {
  let startContent = element.getAttribute("href")!;
  let comments = KoobooComment.getAroundComments(element);
  let comment = comments.find(f => f.getValue("attribute") == "href")!;

  try {
    let url = await createLinkPicker(startContent);
    element.setAttribute("href", url);
    let guid = setGuid(element);
    let unit = new AttributeUnit(startContent, "href");
    let log = [...comment.infos, kvInfo.value(url)];
    let record = new operationRecord([unit], [new Log(log)], guid);
    context.operationManager.add(record);
    return url;
  } catch (error) {
    element.setAttribute("href", startContent);
  }
}

export async function updateUrlLink(element: HTMLElement, koobooId: string, urlComment: KoobooComment, viewComment: KoobooComment) {
  setGuid(element);
  let startContent = element.getAttribute("href")!;

  try {
    let url = await createLinkPicker(startContent);
    element.setAttribute("href", url);
    let guid = setGuid(element);
    let unit = new AttributeUnit(startContent, "href");
    let log = DomLog.createUpdate(viewComment.nameorid!, url, koobooId, viewComment.objecttype!, urlComment.attributename);
    let record = new operationRecord([unit], [log], guid);
    context.operationManager.add(record);
    return url;
  } catch (error) {
    element.setAttribute("href", startContent);
  }
}

export const clearContent = (c: string) => clearKoobooInfo(c).replace(/\s/g, "");

export function isEditable(el: HTMLElement) {
  var reExcept = /^(img|button|input|textarea|hr|area|canvas|meter|progress|select|tr|td|tbody|thead|tfoot|th|table)$/i;
  return !reExcept.test(el.tagName);
}
