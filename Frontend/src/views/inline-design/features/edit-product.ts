import { editProduct } from "@/views/inline-design/commerce";
import { commerceSources, getKoobooBindings, getBinding } from "../binding";
import { i18n } from "@/modules/i18n";

const { t } = i18n.global;

export const name = "editProduct";
export const display = t("commerce.product");
export const icon = "icon-form";
export const order = 0;
export const immediate = true;

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getBinding(bindings, commerceSources);
  return !!binding;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getBinding(bindings, commerceSources)!;
  await editProduct(binding.id);
  location.reload();
}
