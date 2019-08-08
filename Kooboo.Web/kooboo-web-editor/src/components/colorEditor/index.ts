import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getMatchedColorGroups, CssColorGroup } from "@/dom/style";
import { parentBody } from "@/kooboo/outsideInterfaces";
import { createDiv, createCheckboxInput } from "@/dom/element";
import { createColorPicker } from "../common/colorPicker";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { CssUnit } from "@/operation/recordUnits/CssUnit";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { setGuid } from "@/kooboo/utils";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Log } from "@/operation/recordLogs/Log";

export function createColorEditor(el: HTMLElement, comment: KoobooComment, koobooId: string) {
  var startStyle = el.getAttribute("style");
  var groups = getMatchedColorGroups(el);
  let container = createDiv();
  let logGetters: (() => Log[])[] = [];
  for (const i of groups) {
    let { item, getLogs } = createItem(i, el, comment, koobooId);
    logGetters.push(getLogs);
    container.appendChild(item);
  }

  const getUnits = () => [new AttributeUnit(startStyle || "", "style"), new CssUnit("", groups)];

  let { modal, close, setCancelHandler, setOkHandler } = createModal(TEXT.EDIT_COLOR, container, "600px");
  setCancelHandler(() => {
    getUnits().map(m => m.undo(el));
    close();
  });

  setOkHandler(() => {
    let guid = setGuid(el);
    let logs: Log[] = [];
    for (const i of logGetters) {
      logs.push(...i());
    }
    let operation = new operationRecord(getUnits(), logs, guid);
    context.operationManager.add(operation);
    close();
  });
  parentBody.appendChild(modal);
}

function createItem(colorGroup: CssColorGroup, el: HTMLElement, comment: KoobooComment, koobooId: string) {
  let container = createDiv();
  container.style.display = "flex";
  let inlineLog: StyleLog | undefined;
  let cssLog: StyleLog | undefined;
  let color = colorGroup.cssColors[0];
  let globalUpdate = !color.inline;
  let inlineColor = colorGroup.cssColors.find(f => f.inline);
  let cssColor = colorGroup.cssColors.find(f => !f.inline);
  let updateColor = inlineColor || color;
  let label = colorGroup.prop + colorGroup.pseudo;
  let oldValue = color.prop.getColor(color.value);
  let picker = createColorPicker(label, oldValue, s => {
    if (globalUpdate) {
      if (!cssColor) return;
      let important = cssColor.cssStyleRule!.style.getPropertyPriority(cssColor.prop.prop);
      let value = cssColor.prop.replaceColor(cssColor.value, s);
      cssColor.cssStyleRule!.style.setProperty(cssColor.prop.prop, value, important);
      cssColor.newValue = value;
      cssColor.newImportant = important;
      cssLog = StyleLog.createCssUpdate(value, cssColor.prop.prop, cssColor.rawSelector, cssColor.koobooId, cssColor.url!, !!important);
    } else {
      let value = el.style.getPropertyValue(updateColor.prop.prop);
      let important = el.style.getPropertyPriority(updateColor.prop.prop);
      value = updateColor.prop.replaceColor(value, s);
      el.style.setProperty(updateColor.prop.prop, value, important);
      inlineLog = StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, value, updateColor.prop.prop, koobooId);
    }
  });
  picker.style.width = "200px";
  container.appendChild(picker);

  const onChecked = (checked: boolean) => {
    if (!cssColor) return;
    if (checked) {
      let value = el.style.getPropertyValue(updateColor.prop.prop);
      value = updateColor.prop.getColor(value);
      let important = el.style.getPropertyPriority(updateColor.prop.prop);
      value = cssColor.prop.replaceColor(cssColor.value, value);
      cssColor.cssStyleRule!.style.setProperty(cssColor.prop.prop, value, important);
      cssColor.newValue = value;
      cssColor.newImportant = important;
      el.style.removeProperty(updateColor.prop.prop);
      inlineLog = StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, "", updateColor.prop.prop, koobooId);
      cssLog = StyleLog.createCssUpdate(value, cssColor.prop.prop, cssColor.rawSelector, cssColor.koobooId, cssColor.url!, !!important);
      globalUpdate = true;
    } else {
      let important = cssColor.cssStyleRule!.style.getPropertyPriority(cssColor.prop.prop);
      let value = cssColor.cssStyleRule!.style.getPropertyValue(cssColor.prop.prop);
      value = cssColor.prop.getColor(value);
      value = updateColor.prop.replaceColor(updateColor.value, value);
      el.style.setProperty(updateColor.prop.prop, value, important);
      cssColor.cssStyleRule!.style.setProperty(cssColor.prop.prop, cssColor.value, cssColor.important ? "important" : "");
      cssColor.newValue = undefined;
      cssColor.newImportant = undefined;
      cssLog = undefined;
      inlineLog = StyleLog.createUpdate(comment.nameorid!, comment.objecttype!, value, updateColor.prop.prop, koobooId);
      globalUpdate = false;
    }
  };
  let { checkbox } = createCheckboxInput(TEXT.GLOBAL_UPDATE, !color.inline, onChecked);
  if (!cssColor || colorGroup.pseudo) {
    checkbox.style.display = "none";
  }
  container.appendChild(checkbox);

  const getLogs = () => {
    let logs: Log[] = [];
    if (inlineLog) logs.push(inlineLog);
    if (cssLog) logs.push(cssLog);
    return logs;
  };

  return {
    item: container,
    getLogs
  };
}
