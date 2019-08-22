import { createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { createColorPicker } from "../common/colorPicker";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

export function createColor(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string) {
  const container = createDiv();
  const style = getComputedStyle(el);
  let backgroundColorLog: StyleLog | undefined;
  let frontColorLog: StyleLog | undefined;
  let backgroundColorImportant = !!style.getPropertyPriority("background-color");
  let frontColorImportant = !!style.getPropertyPriority("color");

  let bgPicker = createColorPicker(TEXT.BACKGROUND_COLOR, style.backgroundColor!, e => {
    el.style.backgroundColor = e;
    backgroundColorLog = StyleLog.createUpdate(nameOrId, objectType, e, "background-color", koobooId, backgroundColorImportant);
  });
  container.appendChild(bgPicker);

  let frontPicker = createColorPicker(TEXT.COLOR, style.color!, e => {
    el.style.color = e;
    frontColorLog = StyleLog.createUpdate(nameOrId, objectType, e, "color", koobooId, frontColorImportant);
  });
  container.appendChild(frontPicker);

  const getLogs = () => [backgroundColorLog, frontColorLog];
  return { el: container, getLogs };
}
