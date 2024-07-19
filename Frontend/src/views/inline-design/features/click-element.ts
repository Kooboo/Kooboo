import { i18n } from "@/modules/i18n";
import { nextTick } from "vue";
import { leaveTip } from "../page";

const { t } = i18n.global;
export const name = "click";
export const display = t("common.trigger");
export const icon = "icon-Select3";
export const order = 50;

export function active(el: Element) {
  return !!el;
}

export async function invoke(el: HTMLElement) {
  leaveTip.value = t("common.clickLeaveTip");

  setTimeout(() => {
    leaveTip.value = "";
  }, 500);
  await nextTick();
  el.click();
}
