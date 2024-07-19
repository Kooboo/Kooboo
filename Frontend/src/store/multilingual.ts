import { SITE_SELECTED_LANG } from "@/constants/constants";
import { defineStore } from "pinia";
import { useSiteStore } from "./site";
import { i18n } from "@/modules/i18n";

const $t = i18n.global.t;

export const useMultilingualStore = defineStore({
  id: "multilingualStore",
  state: () => {
    return { trigger: 1 };
  },
  getters: {
    visible() {
      const siteStore = useSiteStore();
      const site = siteStore.site;
      return site.enableMultilingual && Object.keys(site.culture).length > 0;
    },
    cultures() {
      const siteStore = useSiteStore();
      return siteStore.site.culture;
    },
    default() {
      const siteStore = useSiteStore();
      return siteStore.site.defaultCulture;
    },
    selected() {
      const siteStore = useSiteStore();
      if (!this.trigger || !siteStore.site) return [];
      return getSelected();
    },
  },
  actions: {
    selectedChanged(lang: string) {
      const siteStore = useSiteStore();
      if (lang === siteStore.site.defaultCulture) return;
      let selected = getSelected();

      if (selected.some((f) => f === lang)) {
        selected = selected.filter((f) => f !== lang);
      } else {
        selected.push(lang);
      }

      localStorage.setItem(
        siteStore.site.id + SITE_SELECTED_LANG,
        JSON.stringify(selected)
      );

      this.trigger++;
    },
    expendAll() {
      for (const key in this.cultures) {
        if (!this.selected.includes(key)) {
          this.selectedChanged(key);
        }
      }
    },
    appendLangText(text: string, lang?: string) {
      const siteStore = useSiteStore();
      const site = siteStore.site;
      const defaultLang = site.defaultCulture;
      if (!site.enableMultilingual) return text;
      let result = `${text} - ${lang ?? defaultLang}`;
      if (!lang || lang == defaultLang) {
        result += ` (${$t("common.default")})`;
      }
      return result;
    },
  },
});

function getSelected() {
  const siteStore = useSiteStore();

  if (!siteStore.site.enableMultilingual) {
    if (siteStore.site.defaultCulture in siteStore.site.culture) {
      [siteStore.site.defaultCulture];
    } else {
      return [
        Object.keys(siteStore.site.culture)[0],
        siteStore.site.defaultCulture,
      ];
    }
  }

  const local = localStorage.getItem(siteStore.site.id + SITE_SELECTED_LANG);
  const selected: string[] = local ? JSON.parse(local) : [];

  if (!selected.some((s) => s === siteStore.site.defaultCulture)) {
    selected.unshift(siteStore.site.defaultCulture);
  }

  return selected.filter((f) => f in siteStore.site.culture);
}
