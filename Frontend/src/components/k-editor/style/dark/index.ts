import contentCss from "./content.min.css";

// 更多语言支持需要手动替换prism.css https://prismjs.com/download.html
import prismCss from "./prism.css";

// 定制 tinymce 样式
import "./skin.scss";

const contents = [
  "html { overflow-x: hidden; } body {font-size: 14px;} .mce-content-body img { max-width: 100%; height: auto; }",
  contentCss.toString(),
  prismCss.toString(),
];

export const style = contents.join("");
