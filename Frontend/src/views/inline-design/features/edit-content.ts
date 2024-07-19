import { editContent } from "@/views/inline-design/content";
import { contentSources, getKoobooBindings, getFirstBinding } from "../binding";
import { i18n } from "@/modules/i18n";

const { t } = i18n.global;

export const name = "editContent";
export const display = t("common.content");
export const icon = "icon-form";
export const order = 0;
export const immediate = true;

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, contentSources, false);
  return !!binding;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, contentSources, false)!;
  await editContent(binding.id);
  location.reload();
}
