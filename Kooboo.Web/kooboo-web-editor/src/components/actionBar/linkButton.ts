import { createButton } from "./button";
import moveIcon from "@/assets/icons/charulianjie.svg";
import { TEXT } from "@/common/lang";
import { createGlobalLinkEditor } from "../globalLinkEditor";

export function createLinkButton(document: Document) {
  var btn = createButton(document, moveIcon, TEXT.EDIT_LINK);
  btn.onclick = createGlobalLinkEditor;
  return btn;
}
