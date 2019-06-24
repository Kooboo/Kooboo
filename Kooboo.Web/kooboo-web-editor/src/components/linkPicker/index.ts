import { createModal } from "../modal";
import { TEXT } from "@/common/lang";
import { getEditorContainer } from "@/dom/utils";

export function createLinkPicker() {
  let { modal, setCancelHandler, setOkHandler } = createModal(
    TEXT.EDIT_LINK,
    "",
    "400px",
    "400px"
  );
  let container = getEditorContainer();
  container.appendChild(modal);
}
