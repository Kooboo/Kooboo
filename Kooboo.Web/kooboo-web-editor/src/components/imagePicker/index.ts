import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";
import { pickImg, parentBody } from "@/kooboo/outsideInterfaces";
import { createDiv, createLabelInput } from "@/dom/element";
import { createImagePreview } from "../common/imagePreview";

export function createImagePicker(img: HTMLImageElement) {
  let container = createDiv();
  let { imagePreview, setImage } = createImagePreview();
  imagePreview.style.margin = "8px auto 16px auto";

  setImage(img.src);
  imagePreview.onclick = () => {
    pickImg(path => {
      setImage(path);
      img.src = path;
    });
  };
  container.appendChild(imagePreview);

  let alt = createLabelInput(TEXT.ALT, "80px");
  alt.setContent(img.alt);
  container.appendChild(alt.input);
  alt.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.alt = e.target.value;
    }
  });

  let title = createLabelInput(TEXT.TITLE, "80px");
  title.setContent(img.title);
  container.appendChild(title.input);
  title.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.title = e.target.value;
    }
  });

  let width = createLabelInput(TEXT.WIDTH, "80px");
  width.input.style.width = "50%";
  width.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.style.width = e.target.value;
    }
  });
  width.setContent(img.style.width!);
  container.appendChild(width.input);

  let height = createLabelInput(TEXT.HEIGHT, "80px");
  height.input.style.width = "50%";
  height.setInputHandler(e => {
    if (e.target instanceof HTMLInputElement) {
      img.style.height = e.target.value;
    }
  });
  height.setContent(img.style.height!);
  container.appendChild(height.input);

  const { modal, setOkHandler, setCancelHandler, close } = createModal(TEXT.EDIT_IMAGE, container, "450px");
  parentBody.appendChild(modal);

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
