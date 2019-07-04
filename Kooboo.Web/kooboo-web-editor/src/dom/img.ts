import { createButton } from "./button";
import { TEXT } from "@/common/lang";

export function createImgPreview(
  showDeleteBtn: boolean = false,
  onDelete?: () => void
) {
  let el = document.createElement("div");
  el.style.outline = "5px solid #eee";
  el.style.margin = "5px";
  el.style.height = "300px";
  el.style.width = "400px";
  el.style.backgroundPosition = "center";
  el.style.backgroundRepeat = "no-repeat";
  el.style.backgroundSize = "contain";
  el.style.position = "relative";
  el.style.cursor = "pointer";

  let button = createButton(TEXT.DELETE);
  button.style.position = "absolute";
  button.style.bottom = "0";
  button.style.right = "0";
  button.style.opacity = "0.8";
  button.style.backgroundColor = "#eee";
  button.style.borderRadius = "24px";
  button.style.visibility = showDeleteBtn ? "visible" : "hidden";
  el.appendChild(button);

  button.onclick = e => {
    el.style.backgroundImage = "";
    if (onDelete) onDelete();
    e.stopPropagation();
  };

  const setImage = (src: string) => {
    if (src.indexOf("url(") > -1) el.style.backgroundImage = src;
    else el.style.backgroundImage = `url('${src}')`;
  };

  return {
    imagePreview: el,
    setImage
  };
}
