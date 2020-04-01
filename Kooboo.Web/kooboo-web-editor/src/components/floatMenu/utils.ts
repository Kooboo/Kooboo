import { KOOBOO_ID } from "@/common/constants";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { operationManager } from "@/operation/Manager";
import { setGuid, clearKoobooInfo, getUnpollutedEl, getWarpContent, isDynamicContent, getHasKoobooIdEl } from "@/kooboo/utils";
import { createImagePicker } from "../imagePicker";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { createLinkPicker } from "../linkPicker";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { isBody } from "@/dom/utils";

export function getRepeatSourceComment(comments: KoobooComment[], source: string | null = null) {
  var repeatComment = comments.find(f => f.source == "repeatitem");
  if (!repeatComment) return;
  for (const commnet of comments) {
    if (commnet.id && (source ? commnet.source == source : true)) {
      let fullpathComment = commnet.getValue("fullpath");
      let alias = repeatComment.getValue("alias");
      if (fullpathComment && alias && fullpathComment.indexOf(alias) > -1) return commnet;
    }
  }
}

export function getEditableComment(comments: KoobooComment[]) {
  if (comments.find(f => f.source == "menu" || f.source == "innerform")) return null;
  return comments.find(f => !f.source.startsWith("repeat") && f.source != "none" && !f.attribute && f.source != "innerform");
}

export function getScopeComment(comments: KoobooComment[]) {
  return comments.find(f => f.scope);
}

export function hasOperation(operationManager: operationManager) {
  return operationManager.previousRecords.length > 0;
}

export async function updateDomImage(element: HTMLImageElement) {
  let { kooobooIdEl, fieldComment, koobooId, scopeComment } = ElementAnalyze(element);
  let startContent = element.cloneNode(true) as HTMLImageElement;
  var hasSrcset = element.hasAttribute("srcset");
  element.removeAttribute("srcset");

  try {
    await createImagePicker(element as HTMLImageElement);
    if (startContent.outerHTML == element.outerHTML) return;
    let guid = setGuid(element);
    let units = [
      new AttributeUnit(startContent.getAttribute("src")!, "src"),
      new AttributeUnit(startContent.getAttribute("style")!, "style"),
      new AttributeUnit(startContent.getAttribute("title")!, "title"),
      new AttributeUnit(startContent.getAttribute("alt")!, "alt")
    ];

    var logs = [];

    if (kooobooIdEl == element && !fieldComment) {
      if (hasSrcset) {
        logs.push(new Log([...scopeComment!.infos, kvInfo.attribute("srcset"), kvInfo.value(""), kvInfo.koobooId(element.getAttribute(KOOBOO_ID))]));
      }

      logs.push(
        new Log([
          ...scopeComment!.infos,
          kvInfo.attribute("src"),
          kvInfo.value(element.getAttribute("src")),
          kvInfo.koobooId(element.getAttribute(KOOBOO_ID))
        ])
      );

      logs.push(
        new Log([
          ...scopeComment!.infos,
          kvInfo.attribute("style"),
          kvInfo.value(element.getAttribute("style")),
          kvInfo.koobooId(element.getAttribute(KOOBOO_ID))
        ])
      );

      logs.push(
        new Log([
          ...scopeComment!.infos,
          kvInfo.attribute("alt"),
          kvInfo.value(element.getAttribute("alt")),
          kvInfo.koobooId(element.getAttribute(KOOBOO_ID))
        ])
      );

      logs.push(
        new Log([
          ...scopeComment!.infos,
          kvInfo.attribute("title"),
          kvInfo.value(element.getAttribute("title")),
          kvInfo.koobooId(element.getAttribute(KOOBOO_ID))
        ])
      );
    } else {
      koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
      let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(element!);
      let comment = fieldComment ? fieldComment : scopeComment;
      logs.push(new Log([...comment!.infos, kvInfo.koobooId(koobooId), kvInfo.value(clearKoobooInfo(content))]));
    }

    let record = new operationRecord(units, logs, guid);
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
  let { kooobooIdEl, fieldComment, koobooId, scopeComment } = ElementAnalyze(element);
  let href = element.getAttribute("href")!;

  try {
    let url = await createLinkPicker(href);
    element.setAttribute("href", url);
    let guid = setGuid(element);
    let unit = new AttributeUnit(href!, "href");
    let logs = [];

    if (kooobooIdEl == element) {
      logs.push(new Log([...scopeComment!.infos, kvInfo.attribute("href"), kvInfo.value(url), kvInfo.koobooId(element.getAttribute(KOOBOO_ID))]));
    } else {
      koobooId = kooobooIdEl ? kooobooIdEl!.getAttribute(KOOBOO_ID) : koobooId;
      let content = kooobooIdEl ? kooobooIdEl.innerHTML : getWarpContent(element!);
      let comment = fieldComment ? fieldComment : scopeComment;
      logs.push(new Log([...comment!.infos, kvInfo.value(clearKoobooInfo(content)), kvInfo.koobooId(koobooId)]));
    }

    let record = new operationRecord([unit], logs, guid);
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

function getFieldComment(comments: KoobooComment[]) {
  for (const iterator of comments) {
    if (iterator.attribute) continue;
    if (iterator.scope) return;
    if (iterator.source == "innerform") continue;
    if (iterator.source.startsWith("repeat") || iterator.source == "none") return;
    return iterator;
  }
}

export function ElementAnalyze(element: HTMLElement) {
  let unpollutedEl = getUnpollutedEl(element);
  let comments = null;
  let operability = false;
  let koobooId = null;
  let fieldComment = null;
  let scopeComment = null;
  let kooobooIdEl = null;
  if (unpollutedEl && !isBody(unpollutedEl)) {
    if (!isDynamicContent(unpollutedEl)) operability = true;
    koobooId = unpollutedEl.getAttribute(KOOBOO_ID);
    comments = KoobooComment.getComments(unpollutedEl);
    if (comments.find(f => f.source == "menu")) operability = false;
    fieldComment = getFieldComment(comments);
    scopeComment = comments.find(f => f.scope);
    kooobooIdEl = getHasKoobooIdEl(unpollutedEl);
    if (kooobooIdEl && isDynamicContent(kooobooIdEl)) kooobooIdEl = null;
  }

  return {
    unpollutedEl,
    comments,
    operability,
    koobooId,
    fieldComment,
    scopeComment,
    kooobooIdEl
  };
}
