import { createLabelInput, createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

export function createSize(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  let style = getComputedStyle(el);
  let widthLog: StyleLog | undefined;
  let heightLog: StyleLog | undefined;
  let widthImportant = rules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority("width"));
  let heightImportant = rules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority("height"));
  let widthImportantStr = widthImportant ? "important" : el.style.getPropertyPriority("width");
  let heightImportantStr = heightImportant ? "important" : el.style.getPropertyPriority("height");

  let width = createLabelInput(TEXT.WIDTH, 90);
  width.input.style.width = "50%";
  width.setContent(style.width!);
  width.setInputHandler(content => {
    el.style.setProperty("width", (content.target! as HTMLInputElement).value, widthImportantStr);
    widthLog = StyleLog.createUpdate(nameOrId, objectType, el.style.width!, "width", koobooId, !!widthImportantStr);
  });
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT, 90);
  height.input.style.width = "50%";
  height.setContent(style.height!);
  height.setInputHandler(content => {
    el.style.setProperty("height", (content.target! as HTMLInputElement).value, heightImportantStr);
    heightLog = StyleLog.createUpdate(nameOrId, objectType, el.style.height!, "height", koobooId, !!heightImportantStr);
  });
  container.appendChild(height.input);

  const getLogs = () => [widthLog, heightLog];
  return { el: container, getLogs };
}
