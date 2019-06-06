let kbLang = localStorage.getItem("lang");
if (!kbLang) kbLang = "en";
export const lang = kbLang;

const en = {
  MENU: "menu"
};

const zh = {
  MENU: "菜单"
} as typeof en;

var langPackages = { en, zh };
export const TEXT = langPackages[
  lang as keyof typeof langPackages
] as typeof en;
