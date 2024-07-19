import { editMenu } from "@/views/inline-design/menu";
import { getKoobooBindings } from "../binding";
import { i18n } from "@/modules/i18n";

const { t } = i18n.global;
export const name = "editMenu";
export const display = t("common.menu");
export const icon = "icon-menu";
export const order = 0;
export const immediate = true;

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  if (!bindings.length) return;
  if (bindings[0].source != "menu") return false;
  return true;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  await editMenu(bindings[0].id);
  location.reload();
}
