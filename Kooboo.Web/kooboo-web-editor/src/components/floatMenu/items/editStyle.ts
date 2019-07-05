import { TEXT } from "@/common/lang";
import { createItem, MenuItem } from "../basic";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import context from "@/common/context";
import { getMenu, getForm, getHtmlBlock, getDelete } from "../utils";
import { createStyleEditor } from "@/components/styleEditor";
import { isBody } from "@/dom/utils";
import { setGuid } from "@/kooboo/utils";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { operationRecord } from "@/operation/Record";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

export function createEditStyleItem(): MenuItem {
  const { el, setVisiable } = createItem(TEXT.EDIT_STYLE, MenuActions.editStyle);

  const update = () => {
    let args = context.lastSelectedDomEventArgs;
    let visiable = true;
    if (isBody(args.element)) visiable = false;
    if (getMenu(args.koobooComments)) visiable = false;
    if (getForm(args.koobooComments)) visiable = false;
    if (getHtmlBlock(args.koobooComments)) visiable = false;
    if (!getDelete(args.koobooComments)) visiable = false;
    setVisiable(visiable);
  };

  el.addEventListener("click", async () => {
    let args = context.lastSelectedDomEventArgs;
    const startContent = args.element.getAttribute("style");
    let comment = getDelete(args.koobooComments)!;
    try {
      let beforeStyle = JSON.parse(JSON.stringify(getComputedStyle(args.element))) as CSSStyleDeclaration;
      await createStyleEditor(args.element);
      let afterStyle = getComputedStyle(args.element);
      let guid = setGuid(args.element);
      let unit = new AttributeUnit(startContent!, "style");
      let logs: StyleLog[] = [];

      const tryAddLog = (before: string, after: string, name: string) => {
        if (before != after) {
          logs.push(StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, after.replace(/"/g, "'"), name, args.koobooId!));
        }
      };

      tryAddLog(beforeStyle.backgroundImage!, afterStyle.backgroundImage!, "background-image");
      tryAddLog(beforeStyle.backgroundColor!, afterStyle.backgroundColor!, "background-color");
      tryAddLog(beforeStyle.color!, afterStyle.color!, "color");
      tryAddLog(beforeStyle.font!, afterStyle.font!, "font");

      if (logs.length == 0) return;
      let record = new operationRecord([unit], logs, guid);
      context.operationManager.add(record);
    } catch (error) {
      if (startContent) args.element.setAttribute("style", startContent);
    }
  });

  return { el, update };
}
