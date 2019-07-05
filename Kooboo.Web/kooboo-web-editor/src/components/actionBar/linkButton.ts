import { createButton } from "./button";
import moveIcon from "@/assets/icons/charulianjie.svg";
import { TEXT } from "@/common/lang";

export function createLinkButton(document: Document) {
  var btn = createButton(document, moveIcon, TEXT.EDIT_LINK);
  return btn;
}
