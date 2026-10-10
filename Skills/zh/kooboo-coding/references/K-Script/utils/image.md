# k.utils.image

> 本地图片格式转换、缩放、水印与元信息

## 概述

`k.utils.image` 通过运行时图片库在本地处理图片字节，不使用远程转换服务。典型流程：`k.file.readBinary` → 处理 → `k.file.writeBinary`。

## TypeScript 定义

```ts
interface KImageUtility {
  changeFormat(binary: number[], newExtension: string): number[];
  resize(image: number[], height: number, width: number): number[] | null;
  addWatermark(
    binary: number[],
    watermark: number[],
    option: WatermarkOptions
  ): number[];
  getSize(image: number[]): SizeMeasurement;
  getFrameCount(image: number[]): number;
  convertToTwoFramesGif(image: number[]): number[] | null;
  changeDpi(file: UploadFile, targetDpi: number, saveAs: string): FileInfo;
  changeDpi(file: FileInfo, targetDpi: number, saveAs?: string): FileInfo;
  resize(
    file: FileInfo,
    height: number,
    width: number,
    saveAs?: string
  ): FileInfo;
}

interface WatermarkOptions {
  align?: string;      // top | center | bottom；默认 center
  justify?: string;    // left | center | right；默认 center
  opacity?: number;    // 默认 0.8
  offsetX?: number;    // 默认 0
  offsetY?: number;    // 默认 0
  repeat?: boolean;    // 默认 false
}

interface SizeMeasurement {
  width: number;
  height: number;
}
```

> `resize` 有字节数组版和 `FileInfo` 版。`changeDpi` 对 `UploadFile` 必须提供 `saveAs`；对 `FileInfo` 可省略，省略时覆盖源文件。

## changeFormat()

将图片转换为指定扩展名。`gif`、`jpg`/`jpeg` 和 `png` 使用对应编码器；其他扩展名（包括 `webp`、`bmp` 和未知值）都会输出 WebP。扩展名可以带或不带开头的点；空白扩展名会抛出异常。

```ts
const src = k.file.readBinary("photo.png");
const webp = k.utils.image.changeFormat(src, "webp");
k.file.writeBinary("photo.webp", webp);
```

## resize()（字节数组）

`resize(image, height, width)` 保持宽高比，将图片限制在目标范围内；不保证输出尺寸恰好等于传入尺寸。某一维为 `0` 或负数时，运行时按比例计算；两维都不为正数时使用 `90 × 90` 的默认范围。参数顺序是先 `height`，后 `width`。

无法读取图片时返回 `null`。如果原图已经小于目标范围，底层缩略图操作会报告无需缩放，当前运行时可能返回空字节数组。

```ts
const src = k.file.readBinary("large.jpg");
const thumb = k.utils.image.resize(src, 200, 200);
if (thumb && thumb.length > 0) {
  k.file.writeBinary("thumb.jpg", thumb);
}
```

### resize()（FileInfo）

直接读写站点 File I/O 文件。源文件不存在时会抛出异常；省略 `saveAs` 时覆盖源文件。当前实现把带前导点的源文件扩展名传给编码器，而编码器的格式判断要求不带点的名称，因此无论源文件或 `saveAs` 使用什么扩展名，输出都可能回退为 WebP。在运行时修复前，不要假设目标后缀与实际字节格式一致。

```ts
const info = k.file.get("uploads/hero.jpg");
k.utils.image.resize(info, 800, 1200, "uploads/hero-800.jpg");
```

## addWatermark()

叠加水印图。`offsetX` 和 `offsetY` 是相对原图宽高的比例。运行时使用 `WatermarkOptions` 中的默认值，但不会校验选项字符串，也不会把数值限制在 `0–1`。

```ts
const image = k.file.readBinary("1.jpg");
const watermark = k.file.readBinary("logo.png");

const output = k.utils.image.addWatermark(image, watermark, {
  align: "bottom",
  justify: "right",
  offsetX: 0.05,
  offsetY: 0.05,
  opacity: 0.8,
});

k.file.writeBinary("output.jpg", output);
```

设置 `repeat: true` 可平铺水印。`align` 使用 `top`、`center` 或 `bottom`，`justify` 使用 `left`、`center` 或 `right`；其他值不会选择文档约定的位置。

## getSize()

读取图片宽高（像素）。对于非 `null` 但无法读取或为空的图片字节，当前运行时返回 `{ width: 0, height: 0 }`；不要把它当作有效尺寸。

```ts
const binary = k.file.readBinary("photo.jpg");
const size = k.utils.image.getSize(binary);
// { width: 1920, height: 1080 }
```

## getFrameCount()

读取图片库报告的帧数。有效的静态单帧 GIF 返回 `1`；无法读取或为空的数据返回 `0`。其他受支持格式取决于图片库提供的页面元信息，不要假设所有静态图片都返回 `1`。

```ts
const binary = k.file.readBinary("anim.webp");
const count = k.utils.image.getFrameCount(binary);
```

## convertToTwoFramesGif()

输入可读取帧数少于两帧时，复制第一帧并返回两帧 GIF。已有至少两帧时，运行时直接返回原始字节，不会把任意多帧动画压缩为两帧。空或无效输入可能返回 `null`。

```ts
const src = k.file.readBinary("anim.gif");
const out = k.utils.image.convertToTwoFramesGif(src);
if (out) {
  k.file.writeBinary("anim-2frame.gif", out);
}
```

## changeDpi()

修改图片 DPI 并保存。`targetDpi` 小于 `72` 时按 `72` 处理。对 `UploadFile`，必须传入上传文件和必填的目标路径 `saveAs`；对 `FileInfo`，`saveAs` 可省略，省略时覆盖源文件。源 `FileInfo` 必须存在。

```ts
const file = k.file.get("print/poster.png");
k.utils.image.changeDpi(file, 300, "print/poster-300dpi.png");
```

## 注意事项

1. **二进制与存储** — 图片方法使用 `number[]`。`k.file.readBinary` 和 `k.file.writeBinary` 操作的是站点 File I/O 根目录，不是媒体库；媒体库 URL 或商品图片 URL 不能直接作为 `readBinary` 路径。
2. **k.file URL** — `writeBinary("verify/photo.webp", bytes)` 后，浏览器访问路径应使用 `k.file.url("verify/photo.webp")`（如 `"/__kb/kfile/verify/photo.webp"`），不是 `"/verify/photo.webp"`。路径概念见[File 模块](../file/index.md)。
3. **失败边界** — 文件不存在时 `readBinary` 可能返回 `null`；未检查前不要把它传给图片方法。传入 `null` 或空字节时，`writeBinary` 返回 `null` 且不会写入文件。
4. **与 converter 区分** — 远程 GIF→WebP 转换使用 [converter.gifToWebp](converter.md)；本地字节转换使用本模块的 `changeFormat`。

## 相关模块

- 文件 API：[File 模块](../file/index.md)
- 远程 GIF→WebP：[converter.md](converter.md)
