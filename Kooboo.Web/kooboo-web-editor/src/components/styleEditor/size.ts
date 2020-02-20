import { createLabelInput, createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { getImportant } from "@/dom/utils";
import { kvInfo } from "@/common/kvInfo";

export function createSize(el: HTMLElement, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  let style = getComputedStyle(el);
  let widthInfos: kvInfo[] | undefined;
  let heightInfos: kvInfo[] | undefined;
  let widthImportant = getImportant(el, "width", rules);
  let heightImportant = getImportant(el, "height", rules);

  let width = createLabelInput(TEXT.WIDTH, 90);
  width.input.style.width = "50%";
  width.setContent(style.width!);
  width.setInputHandler(content => {
    el.style.setProperty("width", (content.target! as HTMLInputElement).value, widthImportant);
    widthInfos = [kvInfo.property("width"), kvInfo.value(el.style.width!), kvInfo.important(widthImportant)];
  });
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT, 90);
  height.input.style.width = "50%";
  height.setContent(style.height!);
  height.setInputHandler(content => {
    el.style.setProperty("height", (content.target! as HTMLInputElement).value, heightImportant);
    heightInfos = [kvInfo.property("height"), kvInfo.value(el.style.height!), kvInfo.important(heightImportant)];
  });
  container.appendChild(height.input);

  const getLogs = () => [widthInfos, heightInfos];
  return { el: container, getLogs };
}
