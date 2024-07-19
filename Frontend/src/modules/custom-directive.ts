import type { Install } from "./types";
import { i18n } from "./i18n";
import { useSiteStore } from "@/store/site";

const $t = i18n.global.t;

export const install: Install = (app) => {
  app.directive("hasPermission", {
    mounted(
      el: any,
      binding: {
        value: {
          feature: string;
          action: string;
          effect?: string;
        };
      }
    ) {
      const { value } = binding;
      if (Object.values(value).some((i) => !i)) {
        return;
      }
      const siteStore = useSiteStore();
      const isDisabled = siteStore.hasAccess(value.feature, value.action);
      if (!isDisabled && el) {
        if (!value.effect) {
          el.setAttribute("title", $t("common.noPermission"));
          el.setAttribute("aria-disabled", true);
          el.setAttribute("disabled", "");
          el.classList.add("is-disabled");
        } else if (value.effect !== "hiddenIcon") {
          el.setAttribute("title", $t("common.noPermission"));
          el.classList.add(
            "opacity-50",
            "cursor-not-allowed",
            "!text-666",
            value.effect === "circle" ? "!bg-[#D8D8D8]" : null
          );
        } else {
          el.classList.add("hidden");
        }
      }
    },
  });
};
