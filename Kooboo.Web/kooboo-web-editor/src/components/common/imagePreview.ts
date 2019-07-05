import { createButton } from "../../dom/button";
import { TEXT } from "@/common/lang";
import rect from "@/assets/img/transparent.png";

export function createImagePreview(showDeleteBtn: boolean = false, onDelete?: () => void) {
  let el = document.createElement("div");
  el.style.outline = "5px solid #eee";
  el.style.margin = "5px";
  el.style.height = "300px";
  el.style.width = "400px";
  el.style.backgroundImage = `url(${rect})`;

  let preview = document.createElement("div");
  preview.style.width = "100%";
  preview.style.height = "100%";
  preview.style.backgroundPosition = "center";
  preview.style.backgroundRepeat = "no-repeat";
  preview.style.backgroundSize = "contain";
  preview.style.position = "relative";
  preview.style.cursor = "pointer";
  el.appendChild(preview);

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
    if (src.indexOf("url(") > -1) preview.style.backgroundImage = src;
    else preview.style.backgroundImage = `url('${src}')`;
  };

  return {
    imagePreview: el,
    setImage
  };
}
