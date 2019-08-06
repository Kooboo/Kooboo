import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getMatchedColors } from "@/dom/style";

export function createColorEditor(el: HTMLElement) {
  console.log(getMatchedColors(el));

  let modal = createModal(TEXT.EDIT_COLOR, "", "600px");
}
