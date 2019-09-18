import { createButton } from "./button";
import moveIcon from "@/assets/icons/xingzhuang-tupian.svg";
import { TEXT } from "@/common/lang";
import { createGlobalImageEditor } from "../globalImageEditor";

export function createImageButton() {
  var btn = createButton(moveIcon, TEXT.EDIT_IMAGES);
  btn.onclick = createGlobalImageEditor;
  return btn;
}
