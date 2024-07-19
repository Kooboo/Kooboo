import { i18n } from "@/modules/i18n";
import { getEditableColors, showColorDialog } from "../color";

const { t } = i18n.global;
export const name = "editColor";
export const display = t("common.color");
export const icon = "icon-background-color";
export const order = 9;

export function active(el: HTMLElement) {
  return getEditableColors(el).length > 0;
}

export function invoke(el: HTMLElement) {
  showColorDialog(el);
}
