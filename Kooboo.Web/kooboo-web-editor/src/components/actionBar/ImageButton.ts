import { createButton } from "./button";
import moveIcon from "@/assets/icons/xingzhuang-tupian.svg";
import { TEXT } from "@/common/lang";
import { createGlobalImageEditor } from "../globalImageEditor";

export function createImageButton(document: Document) {
  var btn = createButton(document, moveIcon, TEXT.EDIT_IMAGE);
  btn.onclick = createGlobalImageEditor;
  return btn;
}
