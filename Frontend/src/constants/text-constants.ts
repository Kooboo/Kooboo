export const CONTROL_TYPE_TEXT = {
  textBox: "输入框",
  textArea: "输入区",
  wysiwygEditor: "富文本编辑器",
  richEditor: "富文本编辑器",
  checkBox: "复选框",
  mediaFile: "图片文件",
  file: "文件",
  selection: "选择框",
  dateTime: "日期时间",
  number: "数量",
  radioBox: "单选框",
  email: "邮件",
  password: "密码",
  plainText: "纯文本",
  submitBtn: "提交按钮",
  resetBtn: "重置按钮",
  divider: "分割线",
  submitAndResetBtn: "提交与重置按钮",
  switch: "切换",
  int32: "Int32",
  boolean: "布尔型",
  tinymce: "富文本编辑器",
  dynamicSpec: "动态规格",
  fixedSpec: "固定规格",
};

export const DATABASE_CONTROL_TYPE_TEXT = {
  TextBox: "输入框",
  TextArea: "输入区",
  RichEditor: "富文本编辑器",
  Selection: "选择框",
  CheckBox: "复选框",
  RadioBox: "单选框",
  Boolean: "布尔型",
  DateTime: "日期时间",
  Number: "数字",
};

export const DATABASE_CONTROL_TYPE_GROUP = {
  showInput: ["TextBox", "TextArea", "RichEditor"],
  showSelect: ["Selection", "CheckBox", "RadioBox"],
  showNone: ["Boolean", "DateTime"],
  showNumber: ["Number"],
};

export const DATABASE_TYPE = [
  { text: "sqlite", value: "sqlite" },
  { text: "mysql", value: "mysql" },
  { text: "sqlserve", value: "sqlserve" },
];
