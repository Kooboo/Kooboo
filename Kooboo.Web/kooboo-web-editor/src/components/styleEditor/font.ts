import { createDiv, createLabelInput } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { getImportant } from "@/dom/utils";

export function createFont(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  let style = getComputedStyle(el);
  let fontSizeLog: StyleLog | undefined;
  let fontWeightLog: StyleLog | undefined;
  let fontFamilyLog: StyleLog | undefined;
  let sizeImportant = getImportant(el, "font-size", rules);
  let weightImportant = getImportant(el, "font-weight", rules);
  let familyImportant = getImportant(el, "font-family", rules);

  let size = createLabelInput(TEXT.FONT_SIZE, 90);
  size.input.style.width = "50%";
  size.setContent(style.fontSize!);
  size.setInputHandler(content => {
    el.style.setProperty("font-size", (content.target! as HTMLInputElement).value, sizeImportant);
    fontSizeLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontSize!, "font-size", koobooId, !!sizeImportant);
  });
  container.appendChild(size.input);

  let weight = createLabelInput(TEXT.FONT_WEIGHT, 90);
  weight.input.style.width = "50%";
  weight.setContent(style.fontWeight!);
  weight.setInputHandler(content => {
    el.style.setProperty("font-weight", (content.target! as HTMLInputElement).value, weightImportant);
    fontWeightLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontWeight!, "font-weight", koobooId, !!weightImportant);
  });
  container.appendChild(weight.input);

  let family = createLabelInput(TEXT.FONT_FAMILY, 90);
  family.setContent(style.fontFamily!);
  family.setInputHandler(content => {
    el.style.setProperty("font-family", (content.target! as HTMLInputElement).value, familyImportant);
    fontFamilyLog = StyleLog.createUpdate(nameOrId, objectType, el.style.fontFamily!, "font-family", koobooId, !!familyImportant);
  });
  container.appendChild(family.input);

  const getLogs = () => [fontSizeLog, fontWeightLog, fontFamilyLog];
  return { el: container, getLogs };
}
