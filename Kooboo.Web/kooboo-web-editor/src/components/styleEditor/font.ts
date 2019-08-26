import { createDiv, createLabelInput } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

export function createFont(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  let style = getComputedStyle(el);
  let fontSizeLog: StyleLog | undefined;
  let fontWeightLog: StyleLog | undefined;
  let fontFamilyLog: StyleLog | undefined;
  let fontSizeImportant = rules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority("font-size"));
  let fontWeightImportant = rules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority("font-weight"));
  let fontFamilyImportant = rules.some(s => el.matches(s.cssRule.selectorText) && s.cssRule.style.getPropertyPriority("font-family"));
  let fontSizeImportantStr = fontSizeImportant ? "important" : el.style.getPropertyPriority("font-size");
  let fontWeightImportantStr = fontWeightImportant ? "important" : el.style.getPropertyPriority("font-weight");
  let fontFamilyImportantStr = fontFamilyImportant ? "important" : el.style.getPropertyPriority("font-family");

  let size = createLabelInput(TEXT.FONT_SIZE, 90);
  size.input.style.width = "50%";
  size.setContent(style.fontSize!);
  size.setInputHandler(content => {
    el.style.setProperty("font-size", (content.target! as HTMLInputElement).value, fontSizeImportantStr);
    fontSizeLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontSize!, "font-size", koobooId, !!fontSizeImportantStr);
  });
  container.appendChild(size.input);

  let weight = createLabelInput(TEXT.FONT_WEIGHT, 90);
  weight.input.style.width = "50%";
  weight.setContent(style.fontWeight!);
  weight.setInputHandler(content => {
    el.style.setProperty("font-weight", (content.target! as HTMLInputElement).value, fontWeightImportantStr);
    fontWeightLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontWeight!, "font-weight", koobooId, !!fontWeightImportantStr);
  });
  container.appendChild(weight.input);

  let family = createLabelInput(TEXT.FONT_FAMILY, 90);
  family.setContent(style.fontFamily!);
  family.setInputHandler(content => {
    el.style.setProperty("font-family", (content.target! as HTMLInputElement).value, fontFamilyImportantStr);
    fontFamilyLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontFamily!, "font-family", koobooId, !!fontFamilyImportantStr);
  });
  container.appendChild(family.input);

  const getLogs = () => [fontSizeLog, fontWeightLog, fontFamilyLog];
  return { el: container, getLogs };
}
