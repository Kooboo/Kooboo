import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationManager } from "@/operation/Manager";
import { getAllNode } from "@/dom/utils";
import { setGuid, clearKoobooInfo, getUnpollutedEl } from "@/kooboo/utils";
import { createImagePicker } from "../imagePicker";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { createLinkPicker } from "../linkPicker";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";
import { pickImg } from "@/kooboo/outsideInterfaces";

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

export async function updateAttributeImage(element: HTMLImageElement) {
  let comments = KoobooComment.getAroundComments(element);
  let comment = comments.find(f => f.getValue("attribute") == "src")!;
  let img = element as HTMLImageElement;
  let startContent = img.getAttribute("src")!;
  pickImg(path => {
    img.src = path;
    let guid = setGuid(img);
    let value = img.getAttribute("src")!;
    let unit = new AttributeUnit(startContent, "src");
    let log = [...comment.infos, kvInfo.value(value)];
    let record = new operationRecord([unit], [new Log(log)], guid);
    context.operationManager.add(record);
  });
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

export const clearContent = (c: string) => clearKoobooInfo(c).replace(/\s/g, "");

export function isEditable(el: HTMLElement) {
  var reExcept = /^(img|button|input|textarea|hr|area|canvas|meter|progress|select|tr|td|tbody|thead|tfoot|th|table)$/i;
  return !reExcept.test(el.tagName);
}
