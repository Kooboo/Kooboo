import { getKoobooBindings, getFirstBinding, labelSources } from "../binding";
import { i18n } from "@/modules/i18n";
import { editLabel } from "@/views/inline-design/label";

const { t } = i18n.global;
export const name = "editLabel";
export const display = t("common.label");
export const icon = "icon-a-writein";
export const order = 0;
export const immediate = true;

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, labelSources);
  return !!binding;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, labelSources)!;
  await editLabel(binding.id);
  location.reload();
}
