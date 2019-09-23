import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { createDiv, createLabelInput } from "@/dom/element";
import { createImagePreview, createPickShade } from "../common/imagePreview";
import context from "@/common/context";
import { getCssRules } from "@/dom/style";
import { getImportant } from "@/dom/utils";

export function createImagePicker(img: HTMLImageElement) {
  let container = createDiv();
  let rules = getCssRules();
  let widthImportant = getImportant(img, "width", rules);
  let heightImportant = getImportant(img, "height", rules);
  let { imagePreview, setImage } = createImagePreview();
  imagePreview.appendChild(createPickShade());
  imagePreview.style.margin = "8px auto 16px auto";
  let style = JSON.parse(JSON.stringify(getComputedStyle(img)));
  style.width = style.width == "0px" ? "auto" : style.width;
  style.height = style.height == "0px" ? "auto" : style.height;

  setImage(img.src);
  imagePreview.onclick = () => {
    pickImg(path => {
      setImage(path);
      img.src = path;
      img.style.setProperty("width", style.width, widthImportant);
      img.style.setProperty("height", style.height, heightImportant);
    });
  };
  container.appendChild(imagePreview);

  let alt = createLabelInput(TEXT.ALT);
  alt.setContent(img.alt);
  container.appendChild(alt.input);
  alt.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.alt = e.target.value;
    }
  });

  let title = createLabelInput(TEXT.TITLE);
  title.setContent(img.title);
  container.appendChild(title.input);
  title.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.title = e.target.value;
    }
  });

  let width = createLabelInput(TEXT.WIDTH);
  width.input.style.width = "50%";
  width.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.style.setProperty("width", e.target.value, widthImportant);
      style.width = e.target.value;
    }
  });
  width.setContent(style.width!);
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT);
  height.input.style.width = "50%";
  height.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.style.setProperty("height", e.target.value, heightImportant);
      style.height = e.target.value;
    }
  });
  height.setContent(style.height!);
  container.appendChild(height.input);

  const { modal, setOkHandler, setCancelHandler, close } = createModal(TEXT.EDIT_IMAGE, container, "450px");
  context.container.appendChild(modal);

  return new Promise((rs, rj) => {
    setOkHandler(() => {
      rs();
      close();
    });
    setCancelHandler(() => {
      rj();
      close();
    });
  });
}
