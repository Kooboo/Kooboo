let kbLang = localStorage.getItem("lang");
if (!kbLang) kbLang = "en";
export const lang = kbLang;

const en = {
  MENU: "menu",
  EDIT: "edit",
  COPY: "copy",
  DELETE: "delete",
  EDIT_IMAGE: "edit image",
  EDIT_LINK: "edit link",
  JUMP_LINK: "jump link",
  EDIT_HTML_BLOCK: "edit html blick",
  OK: "ok",
  CANCEL: "cancel",
  PAGE_LINK: "page link",
  OUT_LINK: " out lint"
};

const zh = {
  MENU: "菜单",
  EDIT: "编辑",
  COPY: "复制这块",
  DELETE: "删除",
  EDIT_IMAGE: "编辑图片",
  EDIT_LINK: "编辑链接",
  JUMP_LINK: "跳转链接",
  EDIT_HTML_BLOCK: "编辑HTML代码块",
  CANCEL: "取消",
  OK: "确定",
  PAGE_LINK: "页面链接",
  OUT_LINK: "外部链接"
} as typeof en;

var langPackages = { en, zh };
export const TEXT = langPackages[
  lang as keyof typeof langPackages
] as typeof en;
