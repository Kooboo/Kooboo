# k.utils.image

> Image format, size and watermark

## Overview

`k.utils.image` handles image bytes or site `FileInfo` in a script.

## changeFormat()

Convert the image bytes to the format corresponding to the new extension.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `binary` | `number[]` | Yes | Source image bytes, usually from `k.file.readBinary()` or an uploaded file |
| `newExtension` | `string` | Yes | Target extension, such as `.webp`, `.png`, or `.jpg` |

**Returns:** `number[]`, the converted image bytes.

```ts
const jpg = k.file.readBinary("media/source.jpg")
const webp = k.utils.image.changeFormat(jpg, ".webp")

k.file.writeBinary("media/source.webp", webp)
return k.file.get("media/source.webp")
```

## resize()

| Overload | Description |
|------|------|
| `resize(file, height, width, saveAs?)` | Adjust site files |
| `resize(bytes, height, width)` | Adjust byte array |

| Parameter | Type | Required | Description |
|------|------|------|------|
| `file` | `FileInfo` | Yes | Site file object, usually from `k.file.get()` |
| `bytes` | `number[]` | Yes | Image byte array |
| `height` | `number` | Yes | Target height. In the byte-array overload, pass `0` to calculate it from width |
| `width` | `number` | Yes | Target width. In the byte-array overload, pass `0` to calculate it from height |
| `saveAs` | `string` | No | Site file path to save as; omitted means overwrite the source file |

**Returns:**

| Overload | Return |
|------|------|
| `resize(file, height, width, saveAs?)` | `FileInfo`, the resized site file |
| `resize(bytes, height, width)` | `number[]`, the resized image bytes |

For a site file, load `FileInfo` with `k.file.get()` first:

```ts
const original = k.file.get("media/product.jpg")
const resized = k.utils.image.resize(original, 600, 800, "media/product-800.jpg")

return {
    url: resized.url,
    size: resized.size
}
```

For uploads or bytes returned by another API, use the byte-array overload:

```ts
const file = k.request.files[0]
const thumbnail = k.utils.image.resize(file.bytes, 240, 240)
k.file.writeBinary(`media/thumbs/${file.fileName}`, thumbnail)
```

## changeDpi()

Modify the DPI and save as site file.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `file` | `UploadFile \| FileInfo` | Yes | Uploaded file or site file object |
| `targetDpi` | `number` | Yes | Target DPI; values below 72 are treated as 72 |
| `saveAs` | `string` | Required for `UploadFile`, optional for `FileInfo` | Save path. The `FileInfo` overload overwrites the source file when omitted |

**Returns:** `FileInfo`, the saved site file.

```ts
const source = k.file.get("media/print-cover.jpg")
const output = k.utils.image.changeDpi(source, 300, "media/print-cover-300dpi.jpg")

return {
    path: output.fullName,
    size: output.size
}
```

## addWatermark()

Add watermark to image bytes; `WatermarkOptions` includes `align`, `justify`, `opacity`, `offsetX/Y`, `repeat`.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `binary` | `number[]` | Yes | Base image bytes |
| `watermark` | `number[]` | Yes | Watermark image bytes |
| `option` | `WatermarkOptions` | Yes | Watermark position, opacity, and repeat behavior |

**Returns:** `number[]`, the watermarked image bytes.

### WatermarkOptions

| Property | Type | Description |
|------|------|------|
| `align` | `string` | Vertical position: `top`, `center`, `bottom` |
| `justify` | `string` | Horizontal position: `left`, `center`, `right` |
| `opacity` | `number` | Opacity, usually from `0` to `1` |
| `offsetX` | `number` | Horizontal offset ratio, for example `0.04` means 4% |
| `offsetY` | `number` | Vertical offset ratio |
| `repeat` | `boolean` | Whether to tile the watermark |

```ts
const image = k.file.readBinary("media/product.jpg")
const watermark = k.file.readBinary("media/watermark.png")

const output = k.utils.image.addWatermark(image, watermark, {
    align: "bottom",
    justify: "right",
    offsetX: 0.04,
    offsetY: 0.04,
    opacity: 0.8,
    repeat: false
})

k.file.writeBinary("media/product-watermarked.jpg", output)
```

## getSize() / getFrameCount() / getGifFrameCount() / convertToTwoFramesGif()

Read dimensions, frame number or convert GIF.

| Method | Parameter | Return | Description |
|------|------|------|------|
| `getSize(image)` | `number[]` | `{ width, height }` | Read image dimensions |
| `getFrameCount(image)` | `number[]` | `number` | Read GIF / WebP frame count |
| `getGifFrameCount(image)` | `number[]` | `number` | Older frame-count method; prefer `getFrameCount()` |
| `convertToTwoFramesGif(image)` | `number[]` | `number[]` | Convert to two-frame GIF bytes |

```ts
const image = k.file.readBinary("media/banner.jpg")
const size = k.utils.image.getSize(image)

return {
    width: size.width,
    height: size.height
}
```

`getFrameCount()` reads GIF / WebP frame counts; `convertToTwoFramesGif()` can reduce an animation to a two-frame GIF.

```ts
const animation = k.file.readBinary("media/loading.webp")
const frameCount = k.utils.image.getFrameCount(animation)

if (frameCount > 2) {
    const gif = k.utils.image.convertToTwoFramesGif(animation)
    k.file.writeBinary("media/loading-preview.gif", gif)
}
```

## Related Docs

- [k.media](../media/) — Media library
- [k.file](../file/file.md)
