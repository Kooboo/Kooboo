import { createDiv, createLabelInput } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { getImportant } from "@/dom/utils";
import { kvInfo } from "@/common/kvInfo";

export function createFont(el: HTMLElement, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  let style = getComputedStyle(el);
  let fontSizeInfos: kvInfo[] | undefined;
  let fontWeightInfos: kvInfo[] | undefined;
  let fontFamilyInfos: kvInfo[] | undefined;
  let sizeImportant = getImportant(el, "font-size", rules);
  let weightImportant = getImportant(el, "font-weight", rules);
  let familyImportant = getImportant(el, "font-family", rules);

  let size = createLabelInput(TEXT.FONT_SIZE, 90);
  size.input.style.width = "50%";
  size.setContent(style.fontSize!);
  size.setInputHandler(content => {
    el.style.setProperty("font-size", (content.target! as HTMLInputElement).value, sizeImportant);
    fontSizeInfos = [kvInfo.property("font-size"), kvInfo.value(el.style.fontSize!), kvInfo.important(sizeImportant)];
  });
  container.appendChild(size.input);

  let weight = createLabelInput(TEXT.FONT_WEIGHT, 90);
  weight.input.style.width = "50%";
  weight.setContent(style.fontWeight!);
  weight.setInputHandler(content => {
    el.style.setProperty("font-weight", (content.target! as HTMLInputElement).value, weightImportant);
    fontWeightInfos = [kvInfo.property("font-weight"), kvInfo.value(el.style.fontWeight!), kvInfo.important(weightImportant)];
  });
  container.appendChild(weight.input);

  let family = createLabelInput(TEXT.FONT_FAMILY, 90);
  family.setContent(style.fontFamily!);
  family.setInputHandler(content => {
    el.style.setProperty("font-family", (content.target! as HTMLInputElement).value, familyImportant);
    fontFamilyInfos = [kvInfo.property("font-family"), kvInfo.value(el.style.fontFamily!), kvInfo.important(familyImportant)];
  });
  container.appendChild(family.input);

  const getLogs = () => [fontSizeInfos, fontWeightInfos, fontFamilyInfos];
  return { el: container, getLogs };
}
