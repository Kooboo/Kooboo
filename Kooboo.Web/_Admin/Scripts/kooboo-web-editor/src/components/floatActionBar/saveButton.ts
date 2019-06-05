import saveIcon from "../../assets/icons/baocun.svg";
import { createButton } from "./button";

export function createSaveButton(document: Document) {
  var saveBtn = createButton(document, saveIcon);
  return saveBtn;
}
