import { OBJECT_TYPE } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationManager } from "@/operation/Manager";
import { getAllNode } from "@/dom/utils";
import { setGuid, markDirty, clearKoobooInfo } from "@/kooboo/utils";
import { createImagePicker } from "../imagePicker";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { StyleUnit } from "@/operation/recordUnits/StyleUnit";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { createLinkPicker } from "../linkPicker";
import { Log } from "@/operation/recordLogs/Log";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { createDiv } from "@/dom/element";

export function getEditComment(comments: KoobooComment[]) {
  const editTypes = [OBJECT_TYPE.view, OBJECT_TYPE.page, OBJECT_TYPE.layout, OBJECT_TYPE.content, OBJECT_TYPE.Label];

  for (const i of comments) {
    if (i.objecttype && editTypes.some(s => s == i.objecttype)) return i;
  }
}

export function getViewComment(comments: KoobooComment[]) {
  const editTypes = [OBJECT_TYPE.view, OBJECT_TYPE.page, OBJECT_TYPE.layout];

  for (const i of comments) {
    if (i.objecttype && editTypes.some(s => s == i.objecttype)) return i;
  }
}

export function getAttributeComment(comments: KoobooComment[], name: string = "") {
  for (const i of comments) {
    if (i.objecttype && i.objecttype == OBJECT_TYPE.attribute && (!name || i.attributename == name)) {
      return i;
    }
  }
}

function getObjectType(comments: KoobooComment[], type: string) {
  for (const i of comments) {
    if (i.objecttype && i.objecttype.toLowerCase() == type.toLowerCase()) return i;
  }
}

export function hasOperation(operationManager: operationManager) {
  return operationManager.previousRecords.length > 0;
}

export const getMenuComment = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.menu);

export const getFormComment = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.form);

export const getHtmlBlockComment = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.htmlblock);

export const getRepeatComment = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.contentrepeater);

export const getUrlComment = (comments: KoobooComment[]) => getObjectType(comments, OBJECT_TYPE.Url);

export function changeNameOrId(node: Node, guid: string, oldGuid: string) {
  if (KoobooComment.isComment(node) && node.nodeValue!.indexOf(oldGuid) > -1) {
    node.nodeValue = node.nodeValue!.replace(/--nameorid='.{36,50}?'/, `--nameorid='${guid}'`);
  }
  if (node instanceof HTMLElement) {
    for (const iterator of getAllNode(node)) {
      changeNameOrId(iterator, guid, oldGuid);
    }
  }
}

export async function updateDomImage(element: HTMLImageElement, closeParent: HTMLElement, parentKoobooId: string, comment: KoobooComment) {
  setGuid(closeParent);
  let startContent = element.cloneNode(true) as HTMLImageElement;
  try {
    await createImagePicker(element as HTMLImageElement);
    if (startContent.outerHTML == element.outerHTML) return;
    markDirty(closeParent);
    let guid = setGuid(closeParent);
    let value = clearKoobooInfo(closeParent!.innerHTML);
    let oldSrc = startContent.getAttribute("src");
    let unit = new AttributeUnit(oldSrc!, "src");
    let log = DomLog.createUpdate(comment.nameorid!, value, parentKoobooId!, comment.objecttype!);
    let record = new operationRecord([unit], [log], guid);
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

export async function updateDomLink(closeParent: HTMLElement, parentKoobooId: string, element: HTMLElement, comment: KoobooComment) {
  setGuid(closeParent);
  let startContent = closeParent.innerHTML;
  let href = element.getAttribute("href")!;

  try {
    let url = await createLinkPicker(href);
    element.setAttribute("href", url);
    let guid = setGuid(closeParent);
    let value = clearKoobooInfo(closeParent.innerHTML);
    let unit = new InnerHtmlUnit(startContent);
    let log = DomLog.createUpdate(comment.nameorid!, value, parentKoobooId, comment.objecttype!);
    let record = new operationRecord([unit], [log], guid);
    context.operationManager.add(record);
    return url;
  } catch (error) {
    closeParent.innerHTML = startContent;
  }
}

export async function updateAttributeLink(element: HTMLElement, koobooId: string, comment: KoobooComment) {
  setGuid(element);
  let startContent = element.getAttribute("href")!;

  try {
    let url = await createLinkPicker(startContent);
    element.setAttribute("href", url);
    let guid = setGuid(element);
    let unit = new AttributeUnit(startContent, "href");
    let log: Log;

    if (isViewComment(comment)) {
      log = DomLog.createUpdate(comment.nameorid!, url, koobooId, comment.objecttype!, "href");
    } else {
      log = ContentLog.createUpdate(comment.nameorid!, comment.fieldname!, url);
    }

    let record = new operationRecord([unit], [log], guid);
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

export function getFirstComment(comments: KoobooComment[]) {
  for (const iterator of comments) {
    if (iterator.objecttype == OBJECT_TYPE.Url) continue;
    return iterator;
  }
}

export function isViewComment(comment: KoobooComment) {
  const types = [OBJECT_TYPE.view, OBJECT_TYPE.page, OBJECT_TYPE.layout];
  return types.some(s => s == comment.objecttype);
}

export function isEditComment(comment: KoobooComment) {
  const types = [OBJECT_TYPE.view, OBJECT_TYPE.page, OBJECT_TYPE.layout, OBJECT_TYPE.content, OBJECT_TYPE.Label, OBJECT_TYPE.attribute];
  return types.some(s => s == comment.objecttype);
}

export function isRepeatComment(comment: KoobooComment) {
  return comment.objecttype == OBJECT_TYPE.contentrepeater;
}

export const clearContent = (c: string) => clearKoobooInfo(c).replace(/\s/g, "");
