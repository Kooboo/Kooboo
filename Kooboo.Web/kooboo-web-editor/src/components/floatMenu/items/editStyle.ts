import { TEXT } from "@/common/lang";
import { createItem, MenuItem } from "../basic";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { createStyleEditor } from "@/components/styleEditor";
import { setGuid, isDirty, clearKoobooInfo, getCleanParent } from "@/kooboo/utils";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { operationRecord } from "@/operation/Record";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { getMenuComment, getHtmlBlockComment, getViewComment } from "../utils";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { DomLog } from "@/operation/recordLogs/DomLog";
import { Log } from "@/operation/recordLogs/Log";
import { getBackgroundImage, isImg, getBackgroundColor } from "@/dom/utils";

export function createEditStyleItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_STYLE, MenuActions.editStyle);

  const update = (comments: KoobooComment[]) => {
    setVisiable(true);
    let args = context.lastSelectedDomEventArgs;
    if (isImg(args.element)) return setVisiable(false);
    if (getMenuComment(comments)) return setVisiable(false);
    if (getHtmlBlockComment(comments)) return setVisiable(false);
    if (!getViewComment(comments)) return setVisiable(false);
    if (!args.koobooId) return setVisiable(false);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    let comments = KoobooComment.getComments(args.element);
    let { koobooId, parent } = getCleanParent(args.element);
    const startContent = args.element.getAttribute("style");
    let comment = getViewComment(comments)!;
    try {
      let beforeStyle = JSON.parse(JSON.stringify(args.element.style)) as CSSStyleDeclaration;
      let { imageInBackground } = getBackgroundImage(args.element);
      let { colorInBackground } = getBackgroundColor(args.element);
      await createStyleEditor(args.element);
      let afterStyle = args.element.style;
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

        if (imageInBackground) {
          tryAddLog(beforeStyle.background!, afterStyle.background!, "background");
        } else {
          tryAddLog(beforeStyle.backgroundImage!, afterStyle.backgroundImage!, "background-image");
        }

        if (colorInBackground) {
          tryAddLog(beforeStyle.background!, afterStyle.background!, "background");
        } else {
          tryAddLog(beforeStyle.backgroundColor!, afterStyle.backgroundColor!, "background-color");
        }

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
