import { createImagePreview, createPickShade } from "../common/imagePreview";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { getImportant, clearCssImageWarp } from "@/dom/utils";
import { kvInfo } from "@/common/kvInfo";

export function createImg(el: HTMLElement, rules: { cssRule: CSSStyleRule }[]) {
  let infos: kvInfo[] | undefined;
  const style = getComputedStyle(el);
  let important = getImportant(el, "background-image", rules);

  const changeImg = (path: string) => {
    let stylePath = path == "none" ? path : `url('${path}')`;
    el.style.setProperty("background-image", stylePath, important);
    infos = [kvInfo.property("background-image"), kvInfo.value(stylePath), kvInfo.important(important)];
    setImage(path);
  };

  const { imagePreview, setImage } = createImagePreview(true, () => changeImg("none"));
  imagePreview.appendChild(createPickShade());
  imagePreview.style.marginLeft = "auto";
  imagePreview.style.marginRight = "auto";
  imagePreview.style.marginBottom = "15px";
  let src = clearCssImageWarp(style.backgroundImage!);
  if (src) setImage(src);
  imagePreview.onclick = () => pickImg(changeImg);
  const getLogs = () => [infos];

  return { el: imagePreview, getLogs };
}
