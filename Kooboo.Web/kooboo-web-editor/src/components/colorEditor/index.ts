import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getMatchedColorGroups } from "@/dom/style";
import EditElement from "./EditElement";
import { pickImg, parentBody } from "@/kooboo/outsideInterfaces";

export function createColorEditor(el: HTMLElement) {
  console.log(getMatchedColorGroups(el));

  let editElement = new EditElement(el);

  let modal = createModal(TEXT.EDIT_COLOR, editElement.render(), "600px");

  parentBody.appendChild(modal.modal);

  return new Promise<void>((rs, rj) => {
    modal.setOkHandler(() => {
      rs();
      editElement.ok();
      modal.close();
    });
    modal.setCancelHandler(() => {
      editElement.cancel();
      rj();
      modal.close();
    });
  });
}
