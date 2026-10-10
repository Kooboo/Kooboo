# k.utils.screenShot

> Web page or HTML screenshots (returns image bytes)

## Overview

`k.utils.screenShot` uses the Chrome screenshot service to generate PNG and other binary output. Remote URLs must be publicly accessible; Site pages can be addressed by a local page name or inline HTML.

It returns `number[]`, commonly used with `k.file.writeBinary` or `k.response.binary`.

## TypeScript definition (excerpt)

```ts
interface ScreenShot {
  take(absoluteUrl: string): number[];
  take(absoluteUrl: string, width: number, height: number): number[];
  take(absoluteUrl: string, width: number, height: number, fullPage: boolean): number[];
  take(
    absoluteUrl: string,
    width: number,
    height: number,
    scaleWidth: number,
    scaleHeight: number,
    fullPage: boolean
  ): number[];
  takeLocalPageUrl(localPageRelativeUrl: string): number[];
  takeLocalPageUrl(
    localPageRelativeUrl: string,
    width: number,
    height: number,
    fullPage: boolean
  ): number[];
  takePage(pageName: string): number[];
  takePage(
    pageName: string,
    width: number,
    height: number,
    fullPage: boolean
  ): number[];
  takeFromHTML(htmlCode: string): number[];
  takeFromHTML(
    htmlCode: string,
    width: number,
    height: number,
    fullPage: boolean
  ): number[];
  takeFromHTML(
    htmlCode: string,
    width: number,
    height: number,
    scaleWidth: number,
    scaleHeight: number,
    fullPage: boolean
  ): number[];
}
```

The default viewport is approximately **1366×768** (the single-argument overload).

## take() — remote URL

The target URL must be reachable from the screenshot service (typically a public address).

```ts
const png = k.utils.screenShot.take("https://www.example.com", 1280, 720, true);
k.file.writeBinary("shots/example.png", png);
```

## takePage() / takeLocalPageUrl() — Site pages

```ts
// 按页面名称（Pages 对象名）
const png = k.utils.screenShot.takePage("home", 1280, 720, false);

// 按站点相对 URL
const png2 = k.utils.screenShot.takeLocalPageUrl("/about", 1280, 720, false);
```

## takeFromHTML() — HTML string

```ts
const html = "<html><body><h1>Preview</h1></body></html>";
const png = k.utils.screenShot.takeFromHTML(html, 800, 600, false);
```

## Access URL after writing a file

```ts
const fullName = "shots/page.png";
k.file.writeBinary(fullName, png);
const relativeUrl = k.file.url(fullName); // "/__kb/kfile/shots/page.png"
```

## Notes

1. **Remote limitation** — `take(absoluteUrl)` depends on an externally reachable URL; internal-only addresses may fail.
2. **Local pages** — `takePage` / `takeLocalPageUrl` render in the Site context and do not require public access.
3. **Return value** — Failures may return `null`; check before writing.
