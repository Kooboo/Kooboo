import { defineConfig } from "windicss/helpers";

export default defineConfig({
  theme: {
    colors: {
      blue: "#2296F3", //蓝 透明度统一使用 text-blue/30  ->百分之30透明的蓝
      "blue-10": "#e8f4fe",
      orange: "#FF7148", //橙色
      green: "#50BC76", //绿色
      purple: "#6200EE", //紫色

      fff: "#fff",
      "000": "#000",
      "444": "#444",
      "666": "#666",
      "999": "#999",

      black: "#192845", //主题黑
      gray: "#E9EAF0", //主题灰

      "card-disabled": "#A7B0BE", //离线与停用-卡片
      "list-disabled": "#F8F7F7", //离线与停用-列表
      card: "#FBFBFB", //灰卡片&布局控件BG
      line: "#EBEEF5", //常规分割线
      transparent: "transparent",
    },
    boxShadow: {
      "s-4": "0px 2px 4px 0px rgba(0, 0, 0, 0.04)", //小小阴影 浅
      "s-10": "0px 2px 4px 0px rgba(0, 0, 0, 0.1)", //小阴影 浅
      "s-20": "0px 2px 4px 0px rgba(0, 0, 0, 0.2)", //小阴影 深
      "m-10": "0px 4px 8px 0px rgba(0, 0, 0, 0.1)", //中阴影 浅
      "m-20": "0px 4px 8px 0px rgba(0, 0, 0, 0.2)", //中阴影 深
      "l-10": "0px 8px 16px 0px rgba(0, 0, 0, 0.1)", //大阴影 浅
      "l-20": "0px 8px 16px 0px rgba(0, 0, 0, 0.2)", //大阴影 深
      "light-s-10": "0px 4px 8px 0px rgba(255, 255, 255, 0.1)", //白色小阴影 浅
      none: "none",
    },
    fontWeight: {
      light: 300, //细体
      normal: 400, //正常
      medium: 500, //中等
      bold: 700, //粗体
    },
    fontSize: {
      s: ["12px", "20px"], //小标签
      m: ["14px", "22px"], //正文
      l: ["16px", "24px"], //小标题
      "2l": ["20px", "28px"], //标题
      "3l": ["30px", "38px"], //大标题
      "4l": ["36px", "46px"], //超大标题
    },
    fontFamily: {
      family:
        "PingFangSC, Helvetica neue, Inter, Roboto, Arial, Arial-Regular,Twemoji Country Flags",
      mono: `Monaco, Consolas, "Lucida Console", "Liberation Mono", "DejaVu Sans Mono", "Bitstream Vera Sans Mono", "Courier New", monospace !important`,
    },
    borderRadius: {
      normal: "8px", //常规
      full: "9999px", //药丸/圆形
      "1": "100%", //圆形
    },
    spacing: {
      "4": "4px",
      "8": "8px",
      "12": "12px",
      "16": "16px",
      "24": "24px",
      "32": "32px",
      "64": "64px",
    },
  },
  shortcuts: {
    ellipsis: "whitespace-nowrap overflow-hidden overflow-ellipsis",
  },
});
