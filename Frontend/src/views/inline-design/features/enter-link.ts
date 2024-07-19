import { checkEnterLink } from "@/api/site/inline-design";
import { showConfirm } from "@/components/basic/confirm";
import { errorMessage } from "@/components/basic/message";
import { i18n } from "@/modules/i18n";
import { getElementParents, isTag } from "@/utils/dom";
import { updateQueryString } from "@/utils/url";
import { historyCount } from "../state";
import { useBindingStore } from "@/store/binding";

const { t } = i18n.global;
export const name = "enterLink";
export const display = t("common.enterLink");
export const icon = "icon-link1";
export const order = 5;

export function active(el: HTMLElement) {
  el = getLinkTag(el)!;
  if (!el) return;
  const href = el.getAttribute("href")?.toLowerCase();
  if (!href) return false;

  const bindingStore = useBindingStore();
  if (href.startsWith("http") || href.startsWith("//")) {
    for (const domain of bindingStore.bindings) {
      if (domain.fullName && href.indexOf(domain.fullName) > -1) return true;
    }
    return false;
  }
  return true;
}

export async function invoke(el: HTMLElement) {
  if (historyCount.value > 0) {
    await showConfirm(t("common.hasUnsaveChangeTip"));
  }
  const url = getLinkTag(el)!.getAttribute("href");
  const checkEnterResult = await checkEnterLink(url!);

  if (checkEnterResult.canEnter) {
    location.search = updateQueryString(location.search, {
      path: checkEnterResult.url,
    });
  } else {
    errorMessage(checkEnterResult.message);
  }
}

function getLinkTag(el: HTMLElement) {
  const parents = getElementParents(el, true);
  return parents.find((f) => isTag(f, "a"));
}
