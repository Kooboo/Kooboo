import { createButton } from "./button";
import moveIcon from "@/assets/icons/charulianjie.svg";
import { TEXT } from "@/common/lang";
import { createGlobalLinkEditor } from "../globalLinkEditor";

export function createLinkButton() {
  var btn = createButton(moveIcon, TEXT.EDIT_LINKS);
  btn.onclick = createGlobalLinkEditor;
  return btn;
}
