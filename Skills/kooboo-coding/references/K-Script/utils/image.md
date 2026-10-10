# k.utils.image

> Local image format conversion, resizing, watermarking, and metadata

## Overview

`k.utils.image` processes image bytes locally through the runtime image library; it does not use the remote converter service. The usual flow is `k.file.readBinary` → process → `k.file.writeBinary`.

## TypeScript definition

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
  align?: string;      // top | center | bottom; default center
  justify?: string;    // left | center | right; default center
  opacity?: number;    // default 0.8
  offsetX?: number;    // default 0
  offsetY?: number;    // default 0
  repeat?: boolean;    // default false
}

interface SizeMeasurement {
  width: number;
  height: number;
}
```

> `resize` has a byte-array overload and a `FileInfo` overload. `changeDpi` requires `saveAs` for `UploadFile`; it is optional for `FileInfo` and otherwise overwrites the source file.

## changeFormat()

Convert an image to the requested extension. `gif`, `jpg`/`jpeg`, and `png` use their corresponding encoders. Other extensions, including `webp`, `bmp`, and unknown values, use WebP output. The extension may include or omit the leading dot; a blank extension throws.

```ts
const src = k.file.readBinary("photo.png");
const webp = k.utils.image.changeFormat(src, "webp");
k.file.writeBinary("photo.webp", webp);
```

## resize() (byte array)

`resize(image, height, width)` preserves the aspect ratio and fits the image within the requested bounds; it does not guarantee the exact requested dimensions. If one dimension is `0` or negative, the runtime calculates it proportionally. If both are non-positive, it uses `90 × 90` as the fallback box. The parameter order is `height` first, then `width`.

If the image cannot be read, the method returns `null`. If the source is already smaller than the requested box, the current runtime may return an empty byte array because the underlying thumbnail operation reports that no resize was needed.

```ts
const src = k.file.readBinary("large.jpg");
const thumb = k.utils.image.resize(src, 200, 200);
if (thumb && thumb.length > 0) {
  k.file.writeBinary("thumb.jpg", thumb);
}
```

### resize() (FileInfo)

Read and write a Site File I/O file directly. The source file must exist; otherwise the method throws. If `saveAs` is omitted, the source file is overwritten. The current implementation passes the source extension with its leading dot to the encoder helper, whose format checks expect names without the dot; the output may therefore fall back to WebP regardless of the source or `saveAs` extension. Do not assume the destination suffix matches the bytes until this runtime behavior is fixed.

```ts
const info = k.file.get("uploads/hero.jpg");
k.utils.image.resize(info, 800, 1200, "uploads/hero-800.jpg");
```

## addWatermark()

Overlay a watermark image. `offsetX` and `offsetY` are proportions of the original image width and height. The runtime uses the defaults shown in `WatermarkOptions`; it does not validate the option strings or restrict the numeric values to `0–1`.

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

Set `repeat: true` to tile the watermark. Use `align` values `top`, `center`, or `bottom`, and `justify` values `left`, `center`, or `right`; other values do not select a documented position.

## getSize()

Read the image width and height in pixels. For non-null unreadable or empty image bytes, the current runtime returns `{ width: 0, height: 0 }`; do not treat that as a valid image size.

```ts
const binary = k.file.readBinary("photo.jpg");
const size = k.utils.image.getSize(binary);
// { width: 1920, height: 1080 }
```

## getFrameCount()

Get the frame count reported by the image library. A valid single-frame GIF returns `1`; unreadable or empty data returns `0`. Other supported formats depend on the image library's page metadata, so do not assume every static image returns `1`.

```ts
const binary = k.file.readBinary("anim.webp");
const count = k.utils.image.getFrameCount(binary);
```

## convertToTwoFramesGif()

If the input has fewer than two readable frames, duplicate the first frame and return a two-frame GIF. If it already has at least two frames, the runtime returns the original bytes unchanged; it does not reduce an arbitrary multi-frame animation to two frames. Empty or invalid input may return `null`.

```ts
const src = k.file.readBinary("anim.gif");
const out = k.utils.image.convertToTwoFramesGif(src);
if (out) {
  k.file.writeBinary("anim-2frame.gif", out);
}
```

## changeDpi()

Change the image DPI and save it. A `targetDpi` below `72` is treated as `72`. For `UploadFile`, pass the uploaded file and a required destination `saveAs`; for `FileInfo`, `saveAs` is optional and the source file is overwritten when omitted. The source `FileInfo` must exist.

```ts
const file = k.file.get("print/poster.png");
k.utils.image.changeDpi(file, 300, "print/poster-300dpi.png");
```

## Notes

1. **Binary and storage** — Image methods use `number[]`. `k.file.readBinary` and `k.file.writeBinary` address the Site File I/O root, not the media library; a media-library URL or product-image URL is not a valid `readBinary` path by itself.
2. **k.file URL** — After `writeBinary("verify/photo.webp", bytes)`, use `k.file.url("verify/photo.webp")` for the browser path (for example, `"/__kb/kfile/verify/photo.webp"`), not `"/verify/photo.webp"`. See [File module](../file/index.md) for path concepts.
3. **Failure boundaries** — `readBinary` may return `null` for a missing file; do not pass that value to image methods without checking it. `writeBinary` returns `null` and writes nothing for null or empty bytes.
4. **Difference from converter** — Use [converter.gifToWebp](converter.md) for remote GIF→WebP conversion; use this module's `changeFormat` for local byte conversion.

## Related modules

- File API: [File module](../file/index.md)
- Remote GIF→WebP: [converter.md](converter.md)
