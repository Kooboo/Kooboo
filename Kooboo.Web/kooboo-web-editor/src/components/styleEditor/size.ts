import { createLabelInput, createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { getImportant } from "@/dom/utils";

export function createSize(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  let style = getComputedStyle(el);
  let widthLog: StyleLog | undefined;
  let heightLog: StyleLog | undefined;
  let widthImportant = getImportant(el, "width", rules);
  let heightImportant = getImportant(el, "height", rules);

  let width = createLabelInput(TEXT.WIDTH, 90);
  width.input.style.width = "50%";
  width.setContent(style.width!);
  width.setInputHandler(content => {
    el.style.setProperty("width", (content.target! as HTMLInputElement).value, widthImportant);
    widthLog = StyleLog.createUpdate(nameOrId, objectType, el.style.width!, "width", koobooId, !!widthImportant);
  });
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT, 90);
  height.input.style.width = "50%";
  height.setContent(style.height!);
  height.setInputHandler(content => {
    el.style.setProperty("height", (content.target! as HTMLInputElement).value, heightImportant);
    heightLog = StyleLog.createUpdate(nameOrId, objectType, el.style.height!, "height", koobooId, !!heightImportant);
  });
  container.appendChild(height.input);

  const getLogs = () => [widthLog, heightLog];
  return { el: container, getLogs };
}
