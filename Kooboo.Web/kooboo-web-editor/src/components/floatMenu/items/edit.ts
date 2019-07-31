import { TEXT } from "@/common/lang";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { MenuItem, createItem } from "../basic";
import { isDynamicContent, clearKoobooInfo, getCleanParent, isDirty, markDirty, setGuid } from "@/kooboo/utils";
import { isBody } from "@/dom/utils";
import { setInlineEditor } from "@/components/richEditor";
import { getMenuComment, getEditComment, clearContent } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { InnerHtmlUnit } from "@/operation/recordUnits/InnerHtmlUnit";
import { Log } from "@/operation/recordLogs/Log";
import { OBJECT_TYPE } from "@/common/constants";
import { ContentLog } from "@/operation/recordLogs/ContentLog";
import { LabelLog } from "@/operation/recordLogs/LabelLog";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { operationRecord } from "@/operation/Record";

export function createEditItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT, MenuActions.edit);
  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (isBody(args.element)) return setVisiable(false);
    if (getMenuComment(comments)) return setVisiable(false);
    if (!getEditComment(comments)) return setVisiable(false);
    if (!args.koobooId) return setVisiable(false);
    var reExcept = /^(img|button|input|textarea|hr|area|canvas|meter|progress|select|tr|td|tbody|thead|tfoot|th|table)$/i;
    let el = args.element;
    if (reExcept.test(el.tagName)) return setVisiable(false);
    if (isDynamicContent(args.element)) return setVisiable(false);
  };

  el.addEventListener("click", () => {
    let { element, koobooId } = context.lastSelectedDomEventArgs;
    let startContent = element.innerHTML;
    const onSave = () => {
      if (clearContent(startContent) == clearContent(element.innerHTML)) return;
      let { koobooId: parentKoobooId, parent } = getCleanParent(element);
      let comments = KoobooComment.getComments(element);
      let dirtyEl = parent && isDirty(element) ? parent : element;
      markDirty(dirtyEl);
      koobooId = parentKoobooId && isDirty(element) ? parentKoobooId : koobooId;
      let guid = setGuid(element);
      let units = [new InnerHtmlUnit(startContent)];
      let comment = getEditComment(comments)!;
      let log: Log;

      if (comment.objecttype == OBJECT_TYPE.content) {
        log = ContentLog.createUpdate(comment.nameorid!, comment.fieldname!, clearKoobooInfo(element.innerHTML));
      } else if (comment.objecttype == OBJECT_TYPE.Label) {
        log = LabelLog.createUpdate(comment.bindingvalue!, clearKoobooInfo(element.innerHTML));
      } else {
        log = DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(dirtyEl.innerHTML), koobooId!, comment.objecttype!);
      }

      let operation = new operationRecord(units, [log], guid);
      context.operationManager.add(operation);
    };

    const onCancel = () => {
      element.innerHTML = startContent;
    };
    setInlineEditor({ selector: element, onSave, onCancel });
  });

  return { el, update };
}
