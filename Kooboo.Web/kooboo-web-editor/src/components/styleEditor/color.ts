import { createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { createColorPicker } from "../common/colorPicker";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

export function createColor(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  const style = getComputedStyle(el);
  let backgroundColorLog: StyleLog | undefined;
  let frontColorLog: StyleLog | undefined;
  let frontColorImportant = rules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority("color"));
  let backgroundColorImportant = rules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority("background-color"));
  let backgroundColorImportantStr = backgroundColorImportant ? "important" : el.style.getPropertyPriority("background-color");
  let frontColorImportantStr = frontColorImportant ? "important" : el.style.getPropertyPriority("color");
  let bgPicker = createColorPicker(TEXT.BACKGROUND_COLOR, style.backgroundColor!, e => {
    el.style.setProperty("background-color", e, backgroundColorImportantStr);
    backgroundColorLog = StyleLog.createUpdate(nameOrId, objectType, e, "background-color", koobooId, !!backgroundColorImportantStr);
  });
  container.appendChild(bgPicker);

  let frontPicker = createColorPicker(TEXT.COLOR, style.color!, e => {
    el.style.setProperty("color", e, frontColorImportantStr);
    frontColorLog = StyleLog.createUpdate(nameOrId, objectType, e, "color", koobooId, !!frontColorImportantStr);
  });
  container.appendChild(frontPicker);

  const getLogs = () => [backgroundColorLog, frontColorLog];
  return { el: container, getLogs };
}
