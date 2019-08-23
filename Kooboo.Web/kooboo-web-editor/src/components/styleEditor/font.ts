import { createDiv, createLabelInput } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

export function createFont(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string) {
  const container = createDiv();
  let style = getComputedStyle(el);
  let fontSizeLog: StyleLog | undefined;
  let fontWeightLog: StyleLog | undefined;
  let fontFamilyLog: StyleLog | undefined;
  let fontSizeImportant = el.style.getPropertyPriority("font-size");
  let fontWeightImportant = el.style.getPropertyPriority("font-size");
  let fontFamilyImportant = el.style.getPropertyPriority("font-size");

  let size = createLabelInput(TEXT.FONT_SIZE, 90);
  size.input.style.width = "50%";
  size.setContent(style.fontSize!);
  size.setInputHandler(content => {
    el.style.setProperty("fontSize", (content.target! as HTMLInputElement).value, fontSizeImportant);
    fontSizeLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontSize!, "font-size", koobooId, !!fontSizeImportant);
  });
  container.appendChild(size.input);

  let weight = createLabelInput(TEXT.FONT_WEIGHT, 90);
  weight.input.style.width = "50%";
  weight.setContent(style.fontWeight!);
  weight.setInputHandler(content => {
    el.style.setProperty("fontWeight", (content.target! as HTMLInputElement).value, fontWeightImportant);
    fontWeightLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontWeight!, "font-weight", koobooId, !!fontWeightImportant);
  });
  container.appendChild(weight.input);

  let family = createLabelInput(TEXT.FONT_FAMILY, 90);
  family.setContent(style.fontFamily!);
  family.setInputHandler(content => {
    el.style.setProperty("fontFamily", (content.target! as HTMLInputElement).value, fontFamilyImportant);
    fontFamilyLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontFamily!, "font-family", koobooId, !!fontFamilyImportant);
  });
  container.appendChild(family.input);

  const getLogs = () => [fontSizeLog, fontWeightLog, fontFamilyLog];
  return { el: container, getLogs };
}
