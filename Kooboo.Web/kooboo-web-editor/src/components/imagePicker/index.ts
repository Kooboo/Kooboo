import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { createLabelInput } from "@/dom/input";
import { getEditorContainer } from "@/dom/utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { createImgPreview } from "@/dom/img";

export function createImagePicker(img: HTMLImageElement) {
  let container = document.createElement("div");
  let { imagePreview, setImage } = createImgPreview();
  imagePreview.style.margin = "0 auto 16px auto";

  setImage(img.src);
  imagePreview.onclick = () => {
    pickImg(path => {
      img.src = path;
      setImage(path);
    });
  };
  container.appendChild(imagePreview);

  let alt = createLabelInput(TEXT.ALT, 80, 320);
  alt.setContent(img.alt);
  container.appendChild(alt.input);
  alt.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.alt = e.target.value;
    }
  });

  let title = createLabelInput(TEXT.TITLE, 80, 320);
  title.setContent(img.title);
  container.appendChild(title.input);
  title.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.title = e.target.value;
    }
  });

  let style = getComputedStyle(img);
  let width = createLabelInput(TEXT.WIDTH, 80, 120);
  width.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.style.width = e.target.value;
    }
  });
  width.setContent(style.width!);
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT, 80, 120);
  height.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.style.height = e.target.value;
    }
  });
  height.setContent(style.height!);
  container.appendChild(height.input);

  const { modal, setOkHandler, setCancelHandler, close } = createModal(
    TEXT.EDIT_IMAGE,
    container,
    "450px"
  );
  getEditorContainer().appendChild(modal);

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
