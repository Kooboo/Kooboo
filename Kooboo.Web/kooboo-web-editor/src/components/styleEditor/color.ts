import { createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { createColorPicker } from "../common/colorPicker";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { getImportant } from "@/dom/utils";

export function createColor(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  const style = getComputedStyle(el);
  let backgroundColorLog: StyleLog | undefined;
  let frontColorLog: StyleLog | undefined;
  let bgColorImportant = getImportant(el, "background-color", rules);
  let colorImportant = getImportant(el, "color", rules);
  let bgPicker = createColorPicker(TEXT.BACKGROUND_COLOR, style.backgroundColor!, e => {
    el.style.setProperty("background-color", e, bgColorImportant);
    backgroundColorLog = StyleLog.createUpdate(nameOrId, objectType, e, "background-color", koobooId, !!bgColorImportant);
  });
  container.appendChild(bgPicker);

  let frontPicker = createColorPicker(TEXT.COLOR, style.color!, e => {
    el.style.setProperty("color", e, colorImportant);
    frontColorLog = StyleLog.createUpdate(nameOrId, objectType, e, "color", koobooId, !!colorImportant);
  });
  container.appendChild(frontPicker);

  const getLogs = () => [backgroundColorLog, frontColorLog];
  return { el: container, getLogs };
}
