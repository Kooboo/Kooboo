let kbLang = localStorage.getItem("lang");
if (!kbLang) kbLang = "en";
export const lang = kbLang;

const en = {
  MENU: "menu",
  EDIT: "edit",
  COPY: "copy",
  DELETE: "delete",
  TO_IMAGE: "to image"
};

const zh = {
  MENU: "菜单",
  EDIT: "编辑",
  COPY: "复制这块",
  DELETE: "删除",
  TO_IMAGE: "替换成图片"
} as typeof en;

var langPackages = { en, zh };
export const TEXT = langPackages[
  lang as keyof typeof langPackages
] as typeof en;
