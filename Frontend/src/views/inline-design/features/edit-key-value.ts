import {
  getKoobooBindings,
  getFirstBinding,
  keyValueSources,
} from "../binding";
import { i18n } from "@/modules/i18n";
import { editKeyValue } from "../key-value";

const { t } = i18n.global;
export const name = "editKeyValue";
export const display = t("common.keyValue");
export const icon = "icon-form";
export const order = 0;
export const immediate = true;

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, keyValueSources);
  return !!binding;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, keyValueSources)!;
  await editKeyValue(binding.key!);
  location.reload();
}
