import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { createLabelInput } from "@/dom/input";
import { getEditorContainer } from "@/dom/utils";
import { createImgPreview } from "@/dom/img";
import { pickImg } from "@/common/outsideInterfaces";

export function createImagePicker(img: HTMLImageElement) {
  let container = document.createElement("div");
  let { imagePreview, setImage } = createImgPreview();
  imagePreview.style.margin = "0 auto 16px auto";
  imagePreview.style.cursor = "pointer";
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

  let title = createLabelInput(TEXT.TITLE, 80, 320);
  title.setContent(img.title);
  container.appendChild(title.input);

  let width = createLabelInput(TEXT.WIDTH, 80, 120);
  width.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      let value = Number(e.target.value);
      img.width = value;
    }
  });
  width.setContent(img.width + "");
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT, 80, 120);
  height.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      let value = Number(e.target.value);
      img.height = value;
    }
  });
  height.setContent(img.height + "");
  container.appendChild(height.input);

  const { modal, setOkHandler, setCancelHandler, close } = createModal(
    TEXT.EDIT_IMAGE,
    container,
    "450px"
  );
  getEditorContainer().appendChild(modal);

  return new Promise((rs, rj) => {
    setOkHandler(() => {
      img.alt = alt.getContent();
      img.title = alt.getContent();
      img.width = Number(width.getContent());
      img.height = Number(height.getContent());
      rs();
      close();
    });
    setCancelHandler(() => {
      rj();
      close();
    });
  });
}
