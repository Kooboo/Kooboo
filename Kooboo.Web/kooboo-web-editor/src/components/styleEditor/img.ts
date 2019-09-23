import { createImagePreview, createPickShade } from "../common/imagePreview";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { StyleLog } from "@/operation/recordLogs/StyleLog";
import { getImportant, clearCssImageWarp } from "@/dom/utils";

export function createImg(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string, rules: { cssRule: CSSStyleRule }[]) {
  let log: StyleLog | undefined;
  const style = getComputedStyle(el);
  let important = getImportant(el, "background-image", rules);

  const changeImg = (path: string) => {
    el.style.setProperty("background-image", path, important);
    log = StyleLog.createUpdate(nameOrId, objectType, path, "background-image", koobooId, !!important);
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
  const getLogs = () => [log];

  return { el: imagePreview, getLogs };
}
