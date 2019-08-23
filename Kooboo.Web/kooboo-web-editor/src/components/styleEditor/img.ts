import { createImagePreview } from "../common/imagePreview";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { StyleLog } from "@/operation/recordLogs/StyleLog";

export function createImg(el: HTMLElement, nameOrId: string, objectType: string, koobooId: string) {
  let log: StyleLog | undefined;
  const style = getComputedStyle(el);
  const important = !!el.style.getPropertyPriority("background-image");

  const changeImg = (path: string) => {
    path = path == "none" ? "none" : ` url('${path}')`;
    el.style.backgroundImage = path;
    log = StyleLog.createUpdate(nameOrId, objectType, path, "background-image", koobooId, important);
    setImage(path);
  };

  const { imagePreview, setImage } = createImagePreview(true, () => changeImg("none"));
  imagePreview.style.marginLeft = "auto";
  imagePreview.style.marginRight = "auto";
  imagePreview.style.marginBottom = "15px";
  if (style.backgroundImage) setImage(style.backgroundImage);
  imagePreview.onclick = () => pickImg(changeImg);
  const getLogs = () => [log];

  return { el: imagePreview, getLogs };
}
