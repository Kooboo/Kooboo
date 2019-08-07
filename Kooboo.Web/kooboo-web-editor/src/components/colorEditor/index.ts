import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getMatchedColorGroups } from "@/dom/style";

export function createColorEditor(el: HTMLElement) {
  console.log(getMatchedColorGroups(el));

  let modal = createModal(TEXT.EDIT_COLOR, "", "600px");
}
