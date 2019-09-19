export const EDITOR_SHADE_COLOR = "rgba(0,0,0,0.5)";
export const STANDARD_Z_INDEX = 10000000;
export const HOVER_BORDER_WIDTH = 2;
export const HOVER_BORDER_COLOR = "#1fb5f6a3";
export const SELECTED_BORDER_WIDTH = 1;
export const SELECTED_BORDER_COLOR = "red";
export const HOVER_BORDER_SKIP = "__HOVER_BORDER_SKIP";
export const KOOBOO_ID = "kooboo-id";
export const KOOBOO_DIRTY = "kooboo-dirty";
export const KOOBOO_GUID = "kooboo-guid";
export const EMPTY_COMMENT = "<!--empty-->";
export const BACKGROUND_IMAGE_START = /^\s*url\s*\(\s*[\',\"]?/i;
export const BACKGROUND_IMAGE_END = /[\',\"]?\)\s*$/i;

export const OBJECT_TYPE = {
  content: "content",
  contentrepeater: "contentrepeater",
  htmlblock: "htmlblock",
  Label: "Label",
  view: "view",
  menu: "menu",
  page: "page",
  style: "style", //update image-obsolete
  dom: "dom", //update image-obsolete
  attribute: "attribute",
  layout: "layout",
  form: "form",
  Url: "Url"
};

export const EDITOR_TYPE = {
  dom: "dom",
  htmlblock: "htmlblock",
  label: "label",
  content: "content",
  attribute: "attribute",
  style: "style",
  converter: "converter"
};
