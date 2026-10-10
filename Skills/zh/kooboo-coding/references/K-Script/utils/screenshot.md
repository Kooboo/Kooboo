# k.utils.screenShot

> 网页或 HTML 截图（返回图片字节）

## 概述

`k.utils.screenShot` 通过 Chrome 截图服务生成 PNG 等二进制。远程 URL 须公网可访问；本站页面可用本地页面名或内联 HTML。

返回 `number[]`，常配合 `k.file.writeBinary` 或 `k.response.binary`。

## TypeScript 定义（节选）

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

默认视口约 **1366×768**（单参数重载）。

## take() — 远程 URL

目标 URL 须能从截图服务访问（通常为公网地址）。

```ts
const png = k.utils.screenShot.take("https://www.example.com", 1280, 720, true);
k.file.writeBinary("shots/example.png", png);
```

## takePage() / takeLocalPageUrl() — 本站页面

```ts
// 按页面名称（Pages 对象名）
const png = k.utils.screenShot.takePage("home", 1280, 720, false);

// 按站点相对 URL
const png2 = k.utils.screenShot.takeLocalPageUrl("/about", 1280, 720, false);
```

## takeFromHTML() — HTML 字符串

```ts
const html = "<html><body><h1>Preview</h1></body></html>";
const png = k.utils.screenShot.takeFromHTML(html, 800, 600, false);
```

## 写入文件后的访问 URL

```ts
const fullName = "shots/page.png";
k.file.writeBinary(fullName, png);
const relativeUrl = k.file.url(fullName); // "/__kb/kfile/shots/page.png"
```

## 注意事项

1. **远程限制** — `take(absoluteUrl)` 依赖外部可访问 URL，内网-only 地址可能失败。
2. **本地页面** — `takePage` / `takeLocalPageUrl` 在站点上下文中渲染，无需公网。
3. **返回值** — 失败时可能为 `null`，写入前宜判断。
