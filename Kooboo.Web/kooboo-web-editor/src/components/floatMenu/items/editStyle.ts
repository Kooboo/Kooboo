import { createModal } from "@/components/modal";
import { TEXT } from "@/common/lang";
import { createLabelInput } from "@/dom/input";

export function createEditStyleItem() {
  let { input } = createLabelInput("aa");
  const {} = createModal(TEXT.EDIT_STYLE, input);
}
