import { editConfig } from "@/views/inline-design/config";
import { configSources, getKoobooBindings, getFirstBinding } from "../binding";
import { i18n } from "@/modules/i18n";

const { t } = i18n.global;
export const name = "editConfig";
export const display = t("common.config");
export const icon = "icon-a-setup";
export const order = 0;
export const immediate = true;

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, configSources);
  return !!binding;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, configSources)!;
  await editConfig(binding.key!);
  location.reload();
}
