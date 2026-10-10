# k.utils.image

> 图片格式、尺寸与水印

## 概述

`k.utils.image` 在脚本中处理图片字节或站点 `FileInfo`。

## changeFormat()

将图片字节转为新扩展名对应格式。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `binary` | `number[]` | 是 | 原始图片字节，可来自 `k.file.readBinary()` 或上传文件 |
| `newExtension` | `string` | 是 | 目标格式扩展名，如 `.webp`、`.png`、`.jpg` |

**返回：** `number[]`，转换后的图片字节。

```ts
const jpg = k.file.readBinary("media/source.jpg")
const webp = k.utils.image.changeFormat(jpg, ".webp")

k.file.writeBinary("media/source.webp", webp)
return k.file.get("media/source.webp")
```

## resize()

| 重载 | 说明 |
|------|------|
| `resize(file, height, width, saveAs?)` | 调整站点文件 |
| `resize(bytes, height, width)` | 调整字节数组 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `file` | `FileInfo` | 是 | 站点文件对象，通常来自 `k.file.get()` |
| `bytes` | `number[]` | 是 | 图片字节数组 |
| `height` | `number` | 是 | 目标高度。字节数组重载中传 `0` 时会按宽度等比计算 |
| `width` | `number` | 是 | 目标宽度。字节数组重载中传 `0` 时会按高度等比计算 |
| `saveAs` | `string` | 否 | 站点文件保存路径；不传时覆盖原文件 |

**返回：**

| 重载 | 返回 |
|------|------|
| `resize(file, height, width, saveAs?)` | `FileInfo`，调整后的站点文件 |
| `resize(bytes, height, width)` | `number[]`，调整后的图片字节 |

处理站点文件时，先通过 `k.file.get()` 取得 `FileInfo`：

```ts
const original = k.file.get("media/product.jpg")
const resized = k.utils.image.resize(original, 600, 800, "media/product-800.jpg")

return {
    url: resized.url,
    size: resized.size
}
```

处理上传文件或接口返回的字节时，使用字节数组重载：

```ts
const file = k.request.files[0]
const thumbnail = k.utils.image.resize(file.bytes, 240, 240)
k.file.writeBinary(`media/thumbs/${file.fileName}`, thumbnail)
```

## changeDpi()

修改 DPI 并保存为站点文件。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `file` | `UploadFile \| FileInfo` | 是 | 上传文件或站点文件对象 |
| `targetDpi` | `number` | 是 | 目标 DPI；小于 72 时按 72 处理 |
| `saveAs` | `string` | `UploadFile` 必填，`FileInfo` 可选 | 保存路径。`FileInfo` 重载不传时覆盖原文件 |

**返回：** `FileInfo`，保存后的站点文件。

```ts
const source = k.file.get("media/print-cover.jpg")
const output = k.utils.image.changeDpi(source, 300, "media/print-cover-300dpi.jpg")

return {
    path: output.fullName,
    size: output.size
}
```

## addWatermark()

为图片字节添加水印；`WatermarkOptions` 含 `align`、`justify`、`opacity`、`offsetX/Y`、`repeat`。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `binary` | `number[]` | 是 | 原图字节 |
| `watermark` | `number[]` | 是 | 水印图字节 |
| `option` | `WatermarkOptions` | 是 | 水印位置、透明度与重复方式 |

**返回：** `number[]`，加水印后的图片字节。

### WatermarkOptions

| 属性 | 类型 | 说明 |
|------|------|------|
| `align` | `string` | 垂直位置：`top`、`center`、`bottom` |
| `justify` | `string` | 水平位置：`left`、`center`、`right` |
| `opacity` | `number` | 透明度，通常为 `0` 到 `1` |
| `offsetX` | `number` | 水平方向偏移比例，例如 `0.04` 表示 4% |
| `offsetY` | `number` | 垂直方向偏移比例 |
| `repeat` | `boolean` | 是否平铺水印 |

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

读取尺寸、帧数或转换 GIF。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getSize(image)` | `number[]` | `{ width, height }` | 读取图片尺寸 |
| `getFrameCount(image)` | `number[]` | `number` | 读取 GIF / WebP 帧数 |
| `getGifFrameCount(image)` | `number[]` | `number` | 旧帧数方法；以 `getFrameCount()` 为主 |
| `convertToTwoFramesGif(image)` | `number[]` | `number[]` | 转为两帧 GIF 字节 |

```ts
const image = k.file.readBinary("media/banner.jpg")
const size = k.utils.image.getSize(image)

return {
    width: size.width,
    height: size.height
}
```

`getFrameCount()` 支持 GIF / WebP 帧数读取；`convertToTwoFramesGif()` 可把动图收敛为两帧 GIF。

```ts
const animation = k.file.readBinary("media/loading.webp")
const frameCount = k.utils.image.getFrameCount(animation)

if (frameCount > 2) {
    const gif = k.utils.image.convertToTwoFramesGif(animation)
    k.file.writeBinary("media/loading-preview.gif", gif)
}
```

## 相关文档

- [k.media](../media/) — 媒体库
- [k.file](../file/file.md)
