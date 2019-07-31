import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { MenuItem, createItem } from "../basic";
import { clearKoobooInfo, markDirty, setGuid, getWrapDom } from "@/kooboo/utils";
import { setInlineEditor } from "@/components/richEditor";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { OBJECT_TYPE } from "@/common/constants";
import { createDiv } from "@/dom/element";
import { HtmlblockLog } from "@/operation/recordLogs/HtmlblockLog";
import { operationRecord } from "@/operation/Record";
import { getHtmlBlockComment, clearContent } from "../utils";

export function createInlineEditHtmlBlockItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT, MenuActions.edit);
  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    if (!getHtmlBlockComment(comments)) return setVisiable(false);
  };

  el.addEventListener("click", () => {
    let { element } = context.lastSelectedDomEventArgs;
    let startContent = element.innerHTML;
    const onSave = () => {
      if (clearContent(startContent) == clearContent(element.innerHTML)) return;
      let comments = KoobooComment.getComments(element);
      markDirty(element);
      let guid = setGuid(element);
      let unit = new InnerHtmlUnit(startContent);
      let htmlBlockcomment = getHtmlBlockComment(comments)!;
      let { nodes } = getWrapDom(element, OBJECT_TYPE.htmlblock);
      let temp = createDiv();
      nodes.forEach(i => temp.appendChild(i.cloneNode(true)));
      let log = HtmlblockLog.createUpdate(htmlBlockcomment.nameorid!, clearKoobooInfo(temp.innerHTML));
      let operation = new operationRecord([unit], [log], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      element.innerHTML = startContent;
    };
    setInlineEditor({ selector: element, onSave, onCancel });
  });

  return { el, update };
}
