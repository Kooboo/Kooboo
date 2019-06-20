let kbLang = localStorage.getItem("lang");
if (!kbLang) kbLang = "en";
export const lang = kbLang;

const en = {
  MENU: "menu",
  EDIT: "edit",
  COPY: "copy",
  DELETE: "delete",
  EDIT_IMAGE: "edit image",
  EDIT_LINK: "edit link"
};

const zh = {
  MENU: "菜单",
  EDIT: "编辑",
  COPY: "复制这块",
  DELETE: "删除",
  EDIT_IMAGE: "编辑图片",
  EDIT_LINK: "编辑链接"
} as typeof en;

var langPackages = { en, zh };
export const TEXT = langPackages[
  lang as keyof typeof langPackages
] as typeof en;
