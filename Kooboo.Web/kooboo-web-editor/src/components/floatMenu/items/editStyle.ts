import { TEXT } from "@/common/lang";
import { createItem, MenuItem } from "../basic";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { createStyleEditor } from "@/components/styleEditor";
import { setGuid, isDirty, clearKoobooInfo, getCleanParent } from "@/kooboo/utils";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { operationRecord } from "@/operation/Record";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { getMenuComment, getFormComment, getHtmlBlockComment, getViewComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { Log } from "@/operation/recordLogs/Log";

export function createEditStyleItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_STYLE, MenuActions.editStyle);

  const update = () => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    if (getMenuComment(comments)) return setVisiable(false);
    if (getFormComment(comments)) return setVisiable(false);
    if (getHtmlBlockComment(comments)) return setVisiable(false);
    if (!getViewComment(comments)) return setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    const startContent = args.element.getAttribute("style");
    let comment = getViewComment(comments)!;
    try {
      let beforeStyle = JSON.parse(JSON.stringify(getComputedStyle(args.element))) as CSSStyleDeclaration;
      await createStyleEditor(args.element);
      let afterStyle = getComputedStyle(args.element);
      let guid = setGuid(args.element);
      let unit = new AttributeUnit(startContent!, "style");
      let logs: Log[] = [];

      if (isDirty(args.element)) {
        logs.push(DomLog.createUpdate(comment.nameorid!, clearKoobooInfo(parent!.innerHTML), koobooId!, comment.objecttype!));
      } else {
        const tryAddLog = (before: string, after: string, name: string) => {
          if (before != after) {
            logs.push(StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, after.replace(/"/g, "'"), name, args.koobooId!));
          }
        };

        tryAddLog(beforeStyle.backgroundImage!, afterStyle.backgroundImage!, "background-image");
        tryAddLog(beforeStyle.backgroundColor!, afterStyle.backgroundColor!, "background-color");
        tryAddLog(beforeStyle.color!, afterStyle.color!, "color");
        tryAddLog(beforeStyle.width!, afterStyle.width!, "width");
        tryAddLog(beforeStyle.height!, afterStyle.height!, "height");
        tryAddLog(beforeStyle.font!, afterStyle.font!, "font");
      }

      if (logs.length == 0) return;
      let record = new operationRecord([unit], logs, guid);
      context.operationManager.add(record);
    } catch (error) {
      args.element.setAttribute("style", startContent!);
    }
  });

  return { el, update };
}
