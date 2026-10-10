# k.utils.converter

> HTML / Office / animated-image format conversion

## Overview

`k.utils.converter` calls the Kooboo conversion service to convert HTML to PDF, GIF to WebP, or Office documents to HTML. Returned binary results are usually written with `k.file.writeBinary` or `k.response.binary`.

## TypeScript definition

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

Render an HTML string as a PDF and return the PDF byte array.

Optional `appendTexts` adds header/footer-like text to every page; the placeholders `$pageIndex` and `$pageCount` are supported. `location` is required for each item. `x` and `y` are percentages of the page width and height, and `$pageIndex` starts at `1`. Only `align: "right"` changes the alignment; other values use left alignment.

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

Download directly through an API (when not writing to disk, there is no need to handle a k.file URL):

```ts
k.api.get(() => {
  const pdf = k.utils.converter.htmlToPdf("<h1>Invoice</h1>");
  k.response.binary("application/pdf", pdf, "invoice.pdf");
});
```

## gifToWebp()

Convert GIF binary to WebP binary.

```ts
const gif = k.file.readBinary("anim.gif");
const webp = k.utils.converter.gifToWebp(gif);
k.file.writeBinary("anim.webp", webp);
```

## officeToHTML()

Convert uploaded Office file bytes to HTML; `filename` must include the original extension. On successful conversion, the returned HTML has links corrected to accessible relative paths. Image and CSS entries are stored under the Site's `officeconverter/` path; other converted entries may use their archive paths. The result can be `null` when the conversion service returns no usable HTML.

```ts
const bytes = k.file.readBinary("report.docx");
const html = k.utils.converter.officeToHTML(bytes, "report.docx");
if (html !== null) {
  k.response.write(html);
}
```

## officeToCleanHTML()

Same as `officeToHTML`, but also removes inline styles from `<body>` and is suitable for direct insertion into page content. It can also return `null` when conversion produces no HTML.

```ts
const bytes = k.file.readBinary("report.docx");
const html = k.utils.converter.officeToCleanHTML(bytes, "report.docx");
```

## Notes

1. **Conversion-service dependency** — The Kooboo environment must reach the service configured by `UrlSetting.Converter`; offline or restricted environments may fail. `gifToWebp()` does not guarantee a usable empty result for empty or invalid GIF data; handle conversion failure at the calling boundary.
2. **filename extension** — `officeToHTML` / `officeToCleanHTML` identify the Office type from the filename suffix (such as `.docx` or `.pptx`).
3. **Binary types** — Successful binary inputs and outputs are `number[]`, consistent with `k.file.readBinary`. Invalid or empty GIF data may fail instead of returning an empty array.
4. **Access URL after writing** — The first `writeBinary` argument is a fullName (such as `verify/out.pdf`). For verification or frontend output, use `k.file.url(fullName)` to obtain `"/__kb/kfile/verify/out.pdf"`; the complete address is `站点域名 + relativeUrl`. **Do not** append fullName directly to the domain.

## Related modules

- File I/O: [File module](../file/index.md)
- Local image processing (non-Office): [image.md](image.md)
