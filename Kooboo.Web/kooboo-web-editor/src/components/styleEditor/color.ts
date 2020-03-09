import { createDiv } from "@/dom/element";
import { TEXT } from "@/common/lang";
import { createColorPicker } from "../common/colorPicker";
import { getImportant } from "@/dom/utils";
import { kvInfo } from "@/common/kvInfo";

export function createColor(el: HTMLElement, rules: { cssRule: CSSStyleRule }[]) {
  const container = createDiv();
  const style = getComputedStyle(el);
  let backgroundColorInfos: kvInfo[] | undefined;
  let frontColorInfos: kvInfo[] | undefined;
  let bgColorImportant = getImportant(el, "background-color", rules);
  let colorImportant = getImportant(el, "color", rules);
  let bgPicker = createColorPicker(TEXT.BACKGROUND_COLOR, style.backgroundColor!, e => {
    el.style.setProperty("background-color", e, bgColorImportant);
    backgroundColorInfos = [kvInfo.property("background-color"), kvInfo.value(e), kvInfo.important(bgColorImportant)];
  });
  container.appendChild(bgPicker);

  let frontPicker = createColorPicker(TEXT.COLOR, style.color!, e => {
    el.style.setProperty("color", e, colorImportant);
    frontColorInfos = [kvInfo.property("color"), kvInfo.value(e), kvInfo.important(colorImportant)];
  });
  container.appendChild(frontPicker);

  const getLogs = () => [backgroundColorInfos, frontColorInfos];
  return { el: container, getLogs };
}
