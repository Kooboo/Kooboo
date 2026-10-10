# k.utils.converter

> HTML / Office / 动图格式转换

## 概述

`k.utils.converter` 调用 Kooboo 转换服务，将 HTML 转为 PDF、GIF 转为 WebP，或将 Office 文档转为 HTML。返回的二进制结果通常配合 `k.file.writeBinary` 或 `k.response.binary` 输出。

## TypeScript 定义

```ts
interface Converter {
  htmlToPdf(
    htmlcode: string,
    appendTexts?: AppendText[]
  ): number[];

  gifToWebp(gif: number[]): number[];

  officeToHTML(officebytes: number[], filename: string): string | null;

  officeToCleanHTML(officebytes: number[], filename: string): string | null;
}

interface AppendText {
  text: string;
  color?: { r: number; g: number; b: number };
  location: { x: number; y: number; align?: string };
}
```

## htmlToPdf()

将 HTML 字符串渲染为 PDF，返回 PDF 字节数组。

可选 `appendTexts` 在每页追加页眉/页脚类文字；支持占位符 `$pageIndex`、`$pageCount`。每个元素都必须提供 `location`。`x`、`y` 是页面宽度和高度的百分比；`$pageIndex` 从 `1` 开始。只有 `align: "right"` 会改变对齐方式，其他值按左对齐处理。

```ts
const pdfBytes = k.utils.converter.htmlToPdf("<div>hello</div>", [
  {
    text: "current $pageIndex / total $pageCount",
    color: { r: 255, g: 0, b: 0 },
    location: { x: 90, y: 10, align: "right" },
  },
  {
    text: "kooboo.com",
    color: { r: 255, g: 0, b: 0 },
    location: { x: 10, y: 10 },
  },
]);

k.file.writeBinary("output/1.pdf", pdfBytes);

// 浏览器访问 URL（非 fullName）
const relativeUrl = k.file.url("output/1.pdf");
// → "/__kb/kfile/output/1.pdf"
```

通过 API 直接下载（不落盘时无需关心 k.file URL）：

```ts
k.api.get(() => {
  const pdf = k.utils.converter.htmlToPdf("<h1>Invoice</h1>");
  k.response.binary("application/pdf", pdf, "invoice.pdf");
});
```

## gifToWebp()

将 GIF 二进制转为 WebP 二进制。

```ts
const gif = k.file.readBinary("anim.gif");
const webp = k.utils.converter.gifToWebp(gif);
k.file.writeBinary("anim.webp", webp);
```

## officeToHTML()

上传 Office 文件字节（`filename` 必须带原始扩展名），转换为 HTML。转换成功后，HTML 内的链接会被修正为可访问的相对路径。图片和 CSS 条目会保存到站点的 `officeconverter/` 路径；其他转换条目可能使用其压缩包内的原始路径。转换服务没有返回可用 HTML 时，结果可能为 `null`。

```ts
const bytes = k.file.readBinary("report.docx");
const html = k.utils.converter.officeToHTML(bytes, "report.docx");
if (html !== null) {
  k.response.write(html);
}
```

## officeToCleanHTML()

与 `officeToHTML` 相同，额外清理 `<body>` 上的内联样式，适合直接嵌入页面正文。转换没有生成 HTML 时也可能返回 `null`。

```ts
const bytes = k.file.readBinary("report.docx");
const html = k.utils.converter.officeToCleanHTML(bytes, "report.docx");
```

## 注意事项

1. **依赖转换服务** — 需 Kooboo 环境可访问 `UrlSetting.Converter` 对应服务；离线或受限环境可能失败。`gifToWebp()` 不保证对空或无效 GIF 返回可用的空结果；调用方应在边界处理转换失败。
2. **filename 扩展名** — `officeToHTML` / `officeToCleanHTML` 用文件名后缀识别 Office 类型（如 `.docx`、`.pptx`）。
3. **二进制类型** — 成功转换时，二进制入参/出参为 `number[]`，与 `k.file.readBinary` 一致。空或无效 GIF 数据可能直接失败，而不是返回空数组。
4. **写入后的访问 URL** — `writeBinary` 的第一个参数是 fullName（如 `verify/out.pdf`）。验证或返回给前端时，须用 `k.file.url(fullName)` 得到 `"/__kb/kfile/verify/out.pdf"`，完整地址为 `站点域名 + relativeUrl`。**不要**把 fullName 直接拼到域名后访问。

## 相关模块

- 文件读写：[File 模块](../file/index.md)
- 图片本地处理（非 Office）：[image.md](image.md)
