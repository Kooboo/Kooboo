import {
  getKoobooBindings,
  getFirstBinding,
  htmlblockSources,
} from "../binding";
import { i18n } from "@/modules/i18n";
import { editHtmlblock } from "@/views/inline-design/htmlblock";

const { t } = i18n.global;
export const name = "editHtmlblock";
export const display = t("common.htmlBlock");
export const icon = "icon-html";
export const order = 0;
export const immediate = true;

export function active(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, htmlblockSources);
  return !!binding;
}

export async function invoke(el: Element) {
  const bindings = getKoobooBindings(el);
  const binding = getFirstBinding(bindings, htmlblockSources)!;
  await editHtmlblock(binding.id);
  location.reload();
}
